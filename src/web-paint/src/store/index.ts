import { observable } from "mobx";
import { Color } from "react-color";
import IDrawCommand from "../interfaces/IDrawCommand";
import PenType from "../interfaces/PenType";
import MouseState from "../MouseState";

class Store {
  @observable public drawCommands: IDrawCommand[] = [];
  @observable public mouseState: MouseState;
  @observable public drawType: PenType = PenType.Lines;
  @observable public lineWidth: number = 3;
  @observable public lineColor: string = "red";
  @observable public fillColor: string = "#000";
  @observable public isFileNameDialogShow = false;
  @observable public fileName: string;
}

export default Store;
