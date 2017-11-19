import { Card, Input, message, Modal } from "antd";
import { saveAs } from "file-saver";
import { autorun, IReactionDisposer } from "mobx";
import { observer } from "mobx-react";
import * as React from "react";
import * as ReactDOM from "react-dom";
import EventBus from "../EventBus";
import IDrawCommand from "../interfaces/IDrawCommand";
import IPoint from "../interfaces/IPoint";
import PenType from "../interfaces/PenType";
import MouseState from "../MouseState";
import Store from "../store";

interface ICanvasProps {
  id: string;
  width: number;
  height: number;
  store: Store;
  style: React.CSSProperties;
}

@observer
class Canvas extends React.Component<ICanvasProps> {
  private store: Store;
  private rect: any;
  private prevPath: IPoint[] = [];
  private drawDisposer: IReactionDisposer;

  public constructor(props: ICanvasProps) {
    super(props);
    this.store = props.store;
  }

  public clearCanvas() {
    const { width, height } = this.props;
    this.ctx.fillStyle = "#fff";
    this.ctx.fillRect(0, 0, width, height);
  }

  public clearCommands() {
    this.store.drawCommands = [];
  }

  public undo() {
    this.store.drawCommands.pop();
  }

  public draw(commands: IDrawCommand[]) {
    this.clearCanvas();
    commands.forEach(it => {
      this.drawCommand(it);
    });
  }

  public render() {
    const { width, height, id, style } = this.props;
    return (
      <div>
        <canvas
          id={id}
          width={width}
          height={height}
          style={style}
          onMouseOver={e => this.onMouseEnter(e)}
          onMouseLeave={e => this.onMouseLeave(e)}
          onMouseDown={e => this.onMouseDown(e)}
          onMouseUp={e => this.onMouseUp(e)}
          onMouseMove={e => this.onMouseMove(e)}>
          浏览器不兼容canvas，无法画图
        </canvas>
        <Modal
          title="请输入文件名"
          visible={this.store.isFileNameDialogShow}
          onOk={e => this.onSaveOk(e)}
          onCancel={e => this.onSaveCancel(e)}>
          <Input
            placeholder="请输入文件名"
            addonAfter=".png"
            value={this.store.fileName}
            onChange={e => (this.store.fileName = e.target.value)}
          />
        </Modal>
      </div>
    );
  }

  public componentDidMount() {
    this.drawDisposer = autorun(() => {
      this.draw(this.store.drawCommands);
    });
    EventBus.$on("canvas:clearCommands", this.clearCommands.bind(this));
    EventBus.$on("canvas:undo", this.undo.bind(this));
    EventBus.$on("canvas:save", this.save.bind(this));
  }

  public componentWillUnmount() {
    this.drawDisposer();
  }

  private get Position(): IPoint {
    const rect = ReactDOM.findDOMNode(this.canvas).getBoundingClientRect();
    return { x: rect.left, y: rect.top };
  }

  private get canvas(): HTMLCanvasElement {
    const canvas = document.getElementById(this.props.id);
    if (!canvas) {
      throw new Error("[FATAL] Canvas is null");
    }
    return canvas as HTMLCanvasElement;
  }

  private get ctx(): CanvasRenderingContext2D {
    const ctx = this.canvas.getContext("2d");
    if (!ctx) {
      throw new Error("[FATAL] ctx is null");
    }
    return ctx;
  }

  private drawCommand(command: IDrawCommand) {
    const ctx = this.ctx;
    const { fillColor, lineColor, lineWidth, type, path } = command;

    ctx.beginPath();
    ctx.lineWidth = lineWidth;
    ctx.strokeStyle = lineColor;
    ctx.fillStyle = fillColor;
    switch (type) {
      case PenType.Point: {
        const point = path[0];
        ctx.fillRect(point.x, point.y, 1, 1);
        break;
      }
      case PenType.Line: {
        const start = path[0];
        const end = path[1];
        ctx.moveTo(start.x, start.y);
        ctx.lineTo(end.x, end.y);
        break;
      }
      case PenType.Lines: {
        const fstPoint = path[0];
        ctx.moveTo(fstPoint.x, fstPoint.y);
        let prev = fstPoint;
        let cur: IPoint;
        for (let i = 1; i < path.length; i++) {
          cur = path[i];
          ctx.lineTo(cur.x, cur.y);
          ctx.moveTo(cur.x, cur.y);
          prev = cur;
        }
        break;
      }
      case PenType.Rect: {
        let { x, y } = path[0];
        let x1 = path[1].x;
        let y1 = path[1].y;

        const width = Math.abs(x1 - x);
        const height = Math.abs(y1 - y);

        if (x1 < x && y1 < y) {
          [x, x1] = [x1, x];
          [y, y1] = [y1, y];
        }

        if (x1 < x && y1 > y) {
          [x, y] = [x - width, y];
          [x1, y1] = [x1 + width, y1];
        }

        if (x1 > x && y1 < y) {
          [x, y] = [x, y - height];
          [x1, y1] = [x1, y1 + height];
        }

        ctx.fillStyle = lineColor;
        ctx.fillRect(
          x - lineWidth,
          y - lineWidth,
          width + lineWidth * 2,
          height + lineWidth * 2,
        );
        ctx.fillStyle = fillColor;
        ctx.fillRect(x, y, width, height);

        break;
      }
      case PenType.Circle: {
        const origin = path[0];
        const p = path[1];
        const offsetX = origin.x - p.x;
        const offsetY = origin.y - p.y;
        const radius = Math.sqrt(
          Math.abs(offsetX) ** 2 + Math.abs(offsetY) ** 2,
        );
        ctx.arc(origin.x, origin.y, radius, 0, 2 * Math.PI);
        ctx.fill();
        break;
      }
    }
    ctx.stroke();
  }

  private createDrawCommand(path: IPoint[]): IDrawCommand {
    const clone = (e: string) => JSON.parse(JSON.stringify({ str: e })).str;

    const type = this.store.drawType;
    const lineWidth = this.store.lineWidth;
    const lineColor = clone(this.store.lineColor);
    const fillColor = clone(this.store.fillColor);

    switch (type) {
      case PenType.Line: {
        return {
          type,
          path: [path[0], path[path.length - 1]],
          lineWidth,
          lineColor,
          fillColor,
        };
      }
      case PenType.Lines: {
        if (path.length === 1) {
          return {
            type: PenType.Point,
            path: [path[path.length - 1]],
            lineWidth,
            lineColor,
            fillColor,
          };
        }
        return { type, path, lineWidth, lineColor, fillColor };
      }
      case PenType.Rect: {
        return {
          type,
          path: [path[0], path[path.length - 1]],
          lineWidth,
          lineColor,
          fillColor,
        };
      }
      case PenType.Circle: {
        return {
          type,
          path: [path[0], path[path.length - 1]],
          lineWidth,
          lineColor,
          fillColor,
        };
      }
      default: {
        throw new Error("Cannot reach here");
      }
    }
  }

  private onMouseEnter(e: React.MouseEvent<HTMLCanvasElement>) {
    this.store.mouseState = MouseState.InsideCanvasMouseUp;
  }

  private onMouseLeave(e: React.MouseEvent<HTMLCanvasElement>) {
    this.store.mouseState = MouseState.OutsideCanvas;
  }

  private onMouseDown(e: React.MouseEvent<HTMLCanvasElement>) {
    this.store.mouseState = MouseState.InsideCanvasMouseDown;
  }

  private onMouseUp(e: React.MouseEvent<HTMLCanvasElement>) {
    this.store.mouseState = MouseState.InsideCanvasMouseUp;

    e.persist();
    let { x, y } = this.Position;
    x = e.pageX - x;
    y = e.pageY - y;

    const path = this.prevPath.length === 0 ? [{ x, y }] : this.prevPath;

    const command = this.createDrawCommand(path);
    // 提交画图命令
    this.store.drawCommands.push(command);
    // 清空之前的鼠标路径
    this.prevPath = [];
  }

  private onMouseMove(e: React.MouseEvent<HTMLCanvasElement>) {
    e.persist();

    // 鼠标坐标从屏幕坐标系转换成canvas坐标系
    let { x, y } = this.Position;
    x = e.pageX - x;
    y = e.pageY - y;

    // 鼠标在画布上左键按下并且拖动
    if (this.store.mouseState === MouseState.InsideCanvasMouseDown) {
      this.prevPath.push({ x, y } as IPoint);
      // 预览绘图结果
      const command = this.createDrawCommand(this.prevPath);
      this.draw(this.store.drawCommands);
      this.drawCommand(command);
    }
  }

  private save() {
    this.store.isFileNameDialogShow = true;
  }

  private onSaveCancel(e: React.MouseEvent<any>) {
    this.store.isFileNameDialogShow = false;
  }

  private onSaveOk(e: React.MouseEvent<any>) {
    this.canvas.toBlob(blob => {
      if (blob) {
        saveAs(blob, this.store.fileName);
        this.store.isFileNameDialogShow = false;
      } else {
        message.error("保存失败。将画布转换为文件时出错。");
      }
    });
  }
}

export default Canvas;
