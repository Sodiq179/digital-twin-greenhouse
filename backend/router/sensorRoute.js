const express = require("express");
const router = express.Router();
const sensorController = require("../controllers/sensorController");

router
  .route("/sensors_location_coordinates")
  .get(sensorController.getSensorsLocationCoordinates);

router
  .route("/sensor_data/:year/:month")
  .get(sensorController.getSensorReadings);

router.post("/sensor_data", sensorController.createSensorReadings);

module.exports = router;
