const express = require("express");
const app = express();
const sensorRoute = require("./router/sensorRoute");
const morgan = require("morgan");
const cors = require("cors");
const compression = require("compression");
const globalErrorHandler = require("./controllers/errorController");

if (process.env.NODE_ENV == "development") {
  app.use(morgan("dev"));
}

app.use(express.json());
app.use(cors());
app.use(compression());
app.use(morgan("dev"));

app.use("/sensors", sensorRoute);

app.get("/", (req, res) => {
  res.send("Hello to Clmides API");
});

app.use(globalErrorHandler);

module.exports = app;
