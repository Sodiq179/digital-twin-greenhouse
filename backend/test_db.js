var MongoClient = require("mongodb").MongoClient;

//Create a MongoDB client, open a connection to Amazon DocumentDB as a replica set,
//  and specify the read preference as secondary preferred
var client = MongoClient.connect(
  "mongodb://climdes:t414L2Cz3F(8wc@climdes-db.cdq626omwt2y.us-east-1.docdb.amazonaws.com:27017/?tls=true&tlsCAFile=global-bundle.pem&retryWrites=false",
  {
    useNewUrlParser: true,
  },

  function (err, client) {
    if (err) throw err;
    //Specify the database to be used
    db = client.db("sample-database");

    //Specify the collection to be used
    col = db.collection("sample-collection");

    //Insert a single document
    col.insertOne({ hello: "Amazon DocumentDB" }, function (err, result) {
      //Find the document that was previously written
      col.findOne({ hello: "Amazon DocumentDB" }, function (err, result) {
        //Print the result to the screen
        console.log(result);

        //Close the connection
        client.close();
      });
    });
  }
);
