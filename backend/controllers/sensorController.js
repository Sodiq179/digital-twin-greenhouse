const SensorsLocationCoordinates = require("../models/sensorLocation");
const SensorReadings = require("../models/sensorReading");
const AppError = require("../utils/appError");
const catchAsync = require("../utils/catchAsync");

exports.getSensorsLocationCoordinates = async (req, res) => {
  const query = SensorsLocationCoordinates.find().select("-__v");
  const sensorsLocationCoordinates = await query;
  res.status(200).json({
    status: "success",
    results: sensorsLocationCoordinates.length,
    data: {
      sensorsLocationCoordinates,
    },
  });
};

exports.createSensorReadings = catchAsync(async (req, res, next) => {
  // console.log(req.body);
  const { dateTime, sensorId, temperature, humidityPercent } = req.body;
  if (!dateTime || !sensorId || !temperature || !humidityPercent) {
    return next(
      new AppError(
        "There's a missing parameter. Ensure dateTime, sensorId, temperature, humidityPercent are passed as JSON.",
        400
      )
    );
  }
  let reading;
  try {
    reading = await SensorReadings.findOne({ dateTime });
    if (reading) {
      reading.readings.push({
        dateTime,
        sensorId,
        temperature,
        humidityPercent,
      });
    } else {
      reading = await SensorReadings.create({
        dateTime,
        readings: [{ dateTime, sensorId, temperature, humidityPercent }],
      });
    }
    reading = await reading.save();
  } catch (error) {
    return next(new AppError("Something went wrong", error));
  }
  res.status(201).json({
    status: "success",
    reading,
  });
});

exports.getSensorReadings = catchAsync(async (req, res, next) => {
  const year = parseInt(req.params.year);
  const month = parseInt(req.params.month) - 1;
  const limit = parseInt(req.query.limit) || 1000;
  const page = parseInt(req.query.page) || 1;

  const startDate = new Date(year, month, 1);
  const endDate = new Date(year, month + 1, 1);

  const query = SensorReadings.find({
    dateTime: {
      $gte: startDate,
      $lt: endDate,
    },
    readings: { $exists: true, $ne: [] },
  })
    .select(
      "dateTime readings.sensorId readings.temperature readings.humidityPercent"
    )
    .skip((page - 1) * limit)
    .limit(limit);

  const sensorReadings = await query;

  res.status(200).json({
    status: "success",
    results: sensorReadings.length,
    data: {
      sensorReadings,
    },
  });
});
