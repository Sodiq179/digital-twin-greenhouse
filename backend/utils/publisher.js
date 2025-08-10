const mqtt = require("mqtt");

const options = {
  host: "c1b43953bf324882a20ada72d2f3ca45.s1.eu.hivemq.cloud",
  port: 8883,
  protocol: "mqtts",
  username: "rhedwan",
  password: "4KaPcbB2nKRTM#9",
};

// initialize the MQTT client
const client = mqtt.connect(options);

client.on("connect", function () {
  const topic = "greenhouse/sensor1";
  const message = JSON.stringify({
    sensorId: "A1",
    temperature: 233,
    humidityPercent: 45.2,
    dateTime: "2024-02-09T00:04:00.000Z",
  });
  client.publish(topic, message, () => {
    console.log("Message sent", message);
  });
  client.end();
});

client.on("error", function (error) {
  console.log(error);
});
