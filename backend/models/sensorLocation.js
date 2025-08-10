const mongoose = require("mongoose");

const dataSchema = mongoose.Schema({
  Sensor: {
    type: String,
    required: [true, "Sensor name is required"],
    unique: true,
  },
  X: {
    type: Number,
    required: [true, "X coordinates is required"],
  },
  Y: {
    type: Number,
    required: [true, "Y coordinates is required"],
  },
  Z: {
    type: Number,
    required: [true, "Z coordinates is required"],
  },
});

const SensorsLocationCoordinates = mongoose.model(
  "Sensors Location Coordinates",
  dataSchema
);

module.exports = SensorsLocationCoordinates;
