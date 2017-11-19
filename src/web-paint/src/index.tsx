import "antd/dist/antd.min.css";
import * as React from "react";
import * as ReactDOM from "react-dom";
import App from "./App";
import Store from "./store";

const store = new Store();
const app = <App store={store} />;

ReactDOM.render(app, document.getElementById("app") as HTMLElement);
