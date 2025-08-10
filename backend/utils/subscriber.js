const mqtt = require("mqtt");
const SensorReadings = require("../models/sensorReading");
const dotenv = require("dotenv");
const mongoose = require("mongoose");

dotenv.config({ path: "./config.env" });

let DATABASE = process.env.DATABASE_PRODUCTION;

if (process.env.NODE_ENV === "development") {
  DATABASE = process.env.DATABASE_LOCAL;
}

const port = 4000;

mongoose.connect(DATABASE).then(() => {
  console.log("Database connection is established");
});

const options = {
  host: "c1b43953bf324882a20ada72d2f3ca45.s1.eu.hivemq.cloud",
  port: 8883,
  protocol: "mqtts",
  username: "rhedwan",
  password: "4KaPcbB2nKRTM#9",
};

const client = mqtt.connect(options);

client.on("message", async function (topic, message) {
  try {
    const data = JSON.parse(message.toString());

    if (data.readings && Array.isArray(data.readings)) {
      const newRecord = {
        dateTime: new Date(data.dateTime),
        readings: data.readings.map((reading) => ({
          sensorId: reading.sensorId,
          temperature: reading.temperature,
          humidityPercent: reading.humidityPercent,
          dateTime: new Date(reading.dateTime),
        })),
      };

      await SensorReadings.create(newRecord);
      console.log("Sensor readings saved to database");
    }
  } catch (error) {
    console.error("Error processing message:", error);
  }
});

client.subscribe("greenhouse/#", (err) => {
  if (err) {
    console.error("Failed to subscribe:", err);
  } else {
    console.log("Subscribed to topic: greenhouse/#");
  }
});
