const dotenv = require("dotenv");
const xlsx = require("xlsx");
const mongoose = require("mongoose");
const fs = require("fs").promises; // For async file operations
const path = require("path");

dotenv.config({ path: "./config.env" });

const DATABASE = process.env.DATABASE_PRODUCTION;
const SensorReadings = require("../models/sensorReading");

// Async database connection
async function connectToDatabase() {
  await mongoose.connect(DATABASE);
  console.log("Database Connection is established");
}

async function readXlsxFileAsync(filePath) {
  const buffer = await fs.readFile(filePath);
  const workbook = xlsx.read(buffer, { type: "buffer" });
  const sheetName = workbook.SheetNames[0];
  const worksheet = workbook.Sheets[sheetName];
  return xlsx.utils.sheet_to_json(worksheet);
}

// Adjust the data to match the schema
function processData(data) {
  return data.map((item) => {
    const dateTime = new Date(item["Date/ Time"] + "Z");
    const readings = [];
    Object.keys(item).forEach((key) => {
      const match = key.match(/^([A-Z][1-7])\.\s*(Temp|Humidity)/);
      if (match) {
        const sensorId = match[1];
        const type = match[2];
        let reading = readings.find((r) => r.sensorId === sensorId);
        if (!reading) {
          reading = {
            dateTime,
            sensorId,
            temperature: null,
            humidityPercent: null,
          };
          readings.push(reading);
        }
        if (type === "Temp") {
          reading.temperature = item[key];
        } else if (type.includes("Humidity")) {
          reading.humidityPercent = item[key];
        }
      }
    });
    return { dateTime, readings };
  });
}

async function importData(data) {
  try {
    await SensorReadings.insertMany(data);
    console.log("Data successfully loaded!");
  } catch (err) {
    console.error(err);
  }
}

async function deleteData() {
  try {
    await SensorReadings.deleteMany();
    console.log("Data successfully deleted!");
  } catch (err) {
    console.error(err);
  }
}

async function main() {
  await connectToDatabase();

  const filePath = path.join(
    __dirname,
    "..",
    "csv_data",
    "Smartfarm data_Feb_Winter.xlsx"
  );
  const rawData = await readXlsxFileAsync(filePath);
  const processedData = processData(rawData);

  if (process.argv[2] === "--import") {
    await importData(processedData);
  } else if (process.argv[2] === "--delete") {
    await deleteData();
  }

  // Ensure the script exits cleanly by disconnecting from the database
  await mongoose.disconnect();
  process.exit();
}

main().catch((err) => {
  console.error("An error occurred:", err);
  process.exit(1);
});
