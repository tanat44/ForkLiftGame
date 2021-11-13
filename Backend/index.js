const express = require('express')
const fileUpload = require('express-fileupload')
const cors = require('cors');

const port = 3000
const WEB_DIR = "public"
var fs = require('fs');
var path = require('path');
var ws = require('./websocket')

// MIDDLEWARE
const app = express()
app.use(cors());
app.use(express.json()) 
app.use(express.urlencoded({ extended: true })) 
var apiLog = function (req, res, next) {
  console.log(req.method, req.path)
  next()
}
app.use(apiLog);


// STATIC
app.use('/', express.static(WEB_DIR))

// API 
app.get('/hello', (req, res) => {
  res.json({ok:true, data:'Hello World!'})
})

// SERVER
const server = app.listen(port, () => {
  console.log(`App listening at http://localhost:${port}`)
})
ws.createServer(server)