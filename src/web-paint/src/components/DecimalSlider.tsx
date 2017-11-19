import { Col, InputNumber, Row, Slider } from "antd";
import * as React from "react";

interface IDecimalSliderProps {
  onChange?: (value: number) => void;
  value: number;
}

class DecimalSlider extends React.Component<IDecimalSliderProps> {
  public constructor(props: IDecimalSliderProps) {
    super(props);
  }

  public onChange(value: any) {
    this.setState({ value });
    if (this.props.onChange) {
      this.props.onChange(value);
    }
  }

  public render() {
    return (
      <Row style={{ width: "200px" }}>
        <Col span={12}>
          <Slider
            min={1}
            max={10}
            onChange={e => this.onChange(e)}
            value={this.props.value}
            step={0.01}
          />
        </Col>
        <Col span={4}>
          <InputNumber
            min={1}
            max={10}
            style={{ marginLeft: 8 }}
            step={0.01}
            value={this.props.value}
            onChange={e => this.onChange(e)}
          />
        </Col>
      </Row>
    );
  }
}

export default DecimalSlider;
