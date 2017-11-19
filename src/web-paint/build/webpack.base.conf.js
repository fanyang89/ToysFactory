const webpack = require("webpack");
const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");

const workspaceRoot = path.resolve(path.join(__dirname, "../"));

module.exports = {
  resolve: {
    extensions: [".ts", ".tsx", ".js", ".jsx", ".css", ".less"],
    alias: {
      "@": path.join(workspaceRoot, "src"),
    },
  },
  devtool: "cheap-module-eval-source-map",
  entry: path.join(workspaceRoot, "src/index.tsx"),
  output: {
    path: path.resolve(workspaceRoot, "dist"),
    filename: "[name].js",
    libraryTarget: "umd",
  },
  module: {
    loaders: [
      {
        test: /\.css$/,
        loader: "style-loader!css-loader",
      },
      {
        test: /\.less$/,
        loader: "style-loader!css-loader!less-loader",
      },
      {
        test: /\.tsx?$/,
        loader: "ts-loader",
      },
      {
        test: /\.(png|svg|jpg|gif)$/,
        loader: "file-loader",
        options: {
          name: "[name].[ext]",
          outputPath: "assets/",
        },
      },
      {
        test: /\.(woff|woff2|eot|ttf|otf)$/,
        loader: "file-loader",
        options: {
          name: "[name].[ext]",
          outputPath: "assets/",
        },
      },
    ],
  },
};
