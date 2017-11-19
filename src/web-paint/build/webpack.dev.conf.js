const baseConfig = require("./webpack.base.conf");
const webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const path = require("path");

var devConfig = Object.assign({}, baseConfig);

devConfig.devServer = {
  compress: true,
  port: 9000,
  hot: true,
  lazy: false,
  noInfo: true,
  overlay: true,
  open: true,
};

devConfig.plugins = [
  new webpack.DefinePlugin({
    "process.env": {
      NODE_ENV: JSON.stringify("development"),
    },
  }),
  new webpack.HotModuleReplacementPlugin(),
  new webpack.optimize.CommonsChunkPlugin({
    name: "vendor",
    minChunks: ({ resource }) =>
      resource &&
      resource.indexOf("node_modules") >= 0 &&
      resource.match(/\.js$/),
  }),
  new HtmlWebpackPlugin({
    template: "./src/index.ejs",
    title: "WebPaint",
  }),
];

module.exports = devConfig;
