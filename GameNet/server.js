const client = require('./connection')
const express = require('express')
const app = express()
const bodyParser = require('body-parser')

app.use(bodyParser.json());

app.listen(3000, ()=> {
    console.log("Server is now avai and listening to port %d", 3000)
})

client.connect();

//Display all
app.get('/users', (req, res) =>{
    client.query('Select * from users', (err,result)=>{
        if(!err){
            res.send(result.rows);
        }
    });
    client.end;
})

//Getting one user
app.get('/users/:id', (req, res) =>{
    client.query(`Select * from users where id=${req.params.id}`, (err,result)=>{
        if(!err){
            res.send(result.rows);
        }
    });
    client.end;
})

//Register
app.post('/users', (req, res)=>{
    const user = req.body;
    let insertQuery = `insert into users(name, email, password) 
    values('${user.name}','${user.email}','${user.password}')`;

    client.query(insertQuery, (err, result)=>{
        if(!err){
            res.send('Registration Successful');
        } else {
            console.log(err.message);
        }
    });
    client.end;
})

//Change Password
app.put('/users/:id', (req, res)=>{
    const user = req.body;
    let updateQuery = `update users set name = '${user.name}', 
    email = '${user.email}',
    password = '${user.password}'
    where id='${req.params.id}'`;

    client.query(updateQuery, (err, result)=>{
        if(!err){
            res.send('Password Changed Successfully');
        } else {
            console.log(err.message);
        }
    });
    client.end;
})

//Delet User
app.delete('/users/:id', (req, res) =>{
    let deleteQuery = `delete from users where id=${req.params.id}`;

    client.query(deleteQuery, (err,result)=>{
        if(!err){
            res.send(`Users deleted successfully`);
        } else {
            console.log(err.message);
        }
    });
    client.end;
})
