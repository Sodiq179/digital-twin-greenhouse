const dotenv = require("dotenv");
const xlsx = require("xlsx");
const mongoose = require("mongoose");
const xlsxFile = `${__dirname}/../csv_data/sensors_location_coordinates.xlsx`;
const SensorsLocationCoordinates = require("../models/sensorLocation");


dotenv.config({ path: "./config.env" });
const DATABASE = process.env.DATABASE;
mongoose.connect(DATABASE).then(() => {
  console.log("Database Connection is estabilshed");
});

const workbook = xlsx.readFile(xlsxFile);
const sheetName = workbook.SheetNames[0];
const worksheet = workbook.Sheets[sheetName];
const data = xlsx.utils.sheet_to_json(worksheet);

const importData = async () => {
  try {
    await SensorsLocationCoordinates.create(data);
    console.log("Data successfully loaded!");
  } catch (err) {
    console.log(err);
  }
  process.exit();
};


const deleteData = async () => {
  try {
    await SensorsLocationCoordinates.deleteMany();
    console.log("Data successfully deleted!");
  } catch (err) {
    console.log(err);
  }
  process.exit();
};

if (process.argv[2] === "--import") {
  importData();
} else if (process.argv[2] == "--delete") {
  deleteData();
}

