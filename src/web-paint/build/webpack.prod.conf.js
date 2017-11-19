const HtmlWebpackPlugin = require("html-webpack-plugin");
const webpack = require("webpack");
const baseConfig = require("./webpack.base.conf");

var prodConfig = Object.assign({}, baseConfig);

prodConfig.devtool = "";

prodConfig.plugins = [
  new webpack.DefinePlugin({
    "process.env": {
      NODE_ENV: JSON.stringify("production"),
    },
  }),
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
  new webpack.optimize.UglifyJsPlugin(),
  new webpack.optimize.AggressiveMergingPlugin(),
];

module.exports = prodConfig;
