import { Button, Popover, Radio } from "antd";
import { observer } from "mobx-react";
import * as React from "react";
import { ColorResult, SketchPicker } from "react-color";
import * as ReactDOM from "react-dom";
import DecimalSlider from "../components/DecimalSlider";
import EventBus from "../EventBus";
import PenType from "../interfaces/PenType";
import Store from "../store";
import Style from "./Toolbar.style";

const ButtonGroup = Button.Group;

interface IToolbarProps {
  store: Store;
}

@observer
class Toolbar extends React.Component<IToolbarProps> {
  private store: Store;

  public constructor(props: IToolbarProps) {
    super(props);
    this.store = this.props.store;
  }

  public render() {
    return (
      <div style={{ marginLeft: "50px", lineHeight: "64px" }}>
        <ButtonGroup style={Style.buttonGroup}>
          <Button
            size="large"
            icon="rollback"
            onClick={e => this.onBtnUndoClick(e)}>
            撤销
          </Button>
        </ButtonGroup>

        <ButtonGroup style={Style.buttonGroup}>
          <Radio.Group
            size="large"
            value={this.props.store.drawType}
            onChange={this.onDrawTypeChanged.bind(this)}>
            <Radio.Button value="lines">铅笔</Radio.Button>
            <Radio.Button value="line">直线</Radio.Button>
            <Radio.Button value="rect">矩形</Radio.Button>
            <Radio.Button value="circle">正圆</Radio.Button>
          </Radio.Group>
        </ButtonGroup>

        <ButtonGroup style={Style.buttonGroup}>
          <Popover
            mouseLeaveDelay={0.2}
            placement="bottom"
            content={
              <DecimalSlider
                value={this.store.lineWidth}
                onChange={e => (this.store.lineWidth = e)}
              />
            }
            arrowPointAtCenter>
            <Button size="large">框线宽度</Button>
          </Popover>
          <Popover
            placement="bottom"
            content={
              <SketchPicker
                color={this.store.lineColor}
                onChangeComplete={e => this.onLineColorChange(e)}
              />
            }
            arrowPointAtCenter
            mouseLeaveDelay={0.2}>
            <Button size="large">框线颜色</Button>
          </Popover>
          <Popover
            mouseLeaveDelay={0.2}
            placement="bottom"
            content={
              <SketchPicker
                color={this.store.fillColor}
                onChangeComplete={e => this.onFillColorChange(e)}
              />
            }
            arrowPointAtCenter>
            <Button size="large">填充颜色</Button>
          </Popover>
        </ButtonGroup>

        <ButtonGroup style={Style.buttonGroup}>
          <Button
            onClick={e => this.onBtnClearClick(e)}
            size="large"
            icon="delete">
            清空
          </Button>
        </ButtonGroup>

        <ButtonGroup style={Style.buttonGroup}>
          <Button
            size="large"
            icon="download"
            onClick={e => this.onBtnSaveClick(e)}>
            保存至本地
          </Button>
        </ButtonGroup>
      </div>
    );
  }

  private onLineColorChange(e: ColorResult) {
    this.store.lineColor = e.hex;
  }

  private onFillColorChange(e: ColorResult) {
    this.store.fillColor = e.hex;
  }

  private onBtnClearClick(e: React.FormEvent<any>) {
    EventBus.$emit("canvas:clearCommands");
  }

  private onBtnSaveClick(e: React.FormEvent<any>) {
    EventBus.$emit("canvas:save");
  }

  private onBtnUndoClick(e: React.FormEvent<any>) {
    EventBus.$emit("canvas:undo");
  }

  private onDrawTypeChanged(e: any) {
    this.props.store.drawType = e.target.value;
  }
}

export default Toolbar;
