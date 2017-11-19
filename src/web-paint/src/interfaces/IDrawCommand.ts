import { Color } from "react-color";
import IPoint from "../interfaces/IPoint";
import PenType from "../interfaces/PenType";

interface IDrawCommand {
  type: PenType;
  path: IPoint[];
  lineWidth: number;
  lineColor: string;
  fillColor: string;
}

export default IDrawCommand;
