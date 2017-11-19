import { Breadcrumb, Button, Card, Icon, Layout, Menu } from "antd";
import { observer } from "mobx-react";
import * as Radium from "radium";
import * as React from "react";
import style from "./App.style";
import Canvas from "./components/Canvas";
import Toolbar from "./components/Toolbar";
import Store from "./store";

const { Header, Content, Footer } = Layout;
const ButtonGroup = Button.Group;

interface IAppProps {
  store: Store;
}

@Radium
@observer
class App extends React.Component<IAppProps> {
  public render() {
    const store = this.props.store;
    return (
      <Layout style={{ height: "100vh" }}>
        <Header>
          <img src="assets/logo.png" style={[style.logo]} />
          <span style={{ color: "#fff", fontSize: "26px" }}>WebPaint</span>
        </Header>

        <Content>
          <div style={{ height: "calc(100vh - 128px)" }}>
            <Toolbar store={store} />
            <div
              style={{
                height: "100%",
                width: "100%",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
              }}>
              <Canvas
                id="main"
                width={800}
                height={600}
                store={store}
                style={{
                  boxShadow: "rgba(0, 0, 0, 0.15) 0px 0px 200px",
                }}
              />
            </div>
          </div>
        </Content>
      </Layout>
    );
  }
}

export default App;
