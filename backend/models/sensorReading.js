const mongoose = require("mongoose");

const sensorReadingSchema = new mongoose.Schema({
  sensorId: String,
  temperature: Number,
  humidityPercent: Number,
  dateTime: {
    type: Date,
    required: true,
  },
});

const readingSchema = new mongoose.Schema({
  dateTime: {
    type: Date,
    required: true,
    unique: true,
  },
  readings: [sensorReadingSchema],
});

readingSchema.index({ dateTime: 1 });

const SensorReadings = mongoose.model("SensorReadings", readingSchema);

module.exports = SensorReadings;
