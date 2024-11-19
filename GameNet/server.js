const client = require('./connection')
const express = require('express')
const jwt = require('jsonwebtoken')
const app = express()
const bodyParser = require('body-parser')

app.use(bodyParser.json());

app.listen(3000, ()=> {
    console.log("Server is now avai and listening to port %d", 3000)
})

client.connect();

const JWT_SECRET = 'this-is-secret-token'

const authenticateToken = (req, res, next) =>{
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    if(!token){
        return res.status(401).send('Access token required');
    }
    jwt.verify(token, JWT_SECRET, (err, user)=>{
        if(err){
            return res.status(403).send('Invalid or expiured token');
        }else{
            req.user = user;
            next();
        }
    })
}

app.get('/users', authenticateToken, (req, res) =>{
    client.query('Select * from users', (err,result)=>{
        if(!err){
            res.send(result.rows);
        }
    });
    client.end;
})

//Getting one user
app.get('/users/email/:email', (req, res) => {
    const email = req.params.email;
    client.query(`SELECT * FROM users WHERE email='${email}'`, (err, result) => {
        if (!err) {
            res.send(result.rows);
        } else {
            console.log(err.message);
        }
    });
    client.end;
});

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

//Change Content
app.put('/users/:id', (req, res)=>{
    const { password } = req.body;
    if (!password) {
        res.status(400).send("Password is required.");
        return;
    }

    const updateQuery = `
        UPDATE users
        SET password = '${password}'
        WHERE id = '${req.params.id}'`;

    client.query(updateQuery, (err, result) => {
        if (!err) {
            res.send('Password Changed Successfully');
        } else {
            console.log(err.message);
            res.status(500).send("Failed to update password.");
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

//
app.post('/register', (req, res) =>{
    try {
        const {name, email, password} = req.body;
        if(!email || !name || !password)
            return res.send('Missing one or more fields');

        client.query(`SELECT * from users where name = '${name}'`, (err, result)=>{
            if(err){
                console.log(err.message);
            }else{
                if(result.rows.length > 0){
                    return res.send('User already exist');
                }
            }
        });

        let insertQuery = `insert into users(name, email, password) 
            values('${name}','${email}','${password}')`

        client.query(insertQuery, (err, result)=>{
            if(!err){
                res.send('User registered succesfully')
            }else{
                console.log(err.message);
            }
        });

    }catch (error){
        console.error('Regsiter Error:', error);
        res.send(500).json({error: 'Server error'});
    }
})

app.post('/login', (req, res)=>{
    try {
        const {name, password}= req.body;
        if(!password || !name){
            return res.send('Missing Fields')
        }

        let passwordQuery = `SELECT * from users where name = '${name}' AND password = '${password}'`
        client.query(passwordQuery, (err, result)=>{
            if(!err){
                if(result.rows === 0){
                    return res.status(401).send('Invalid Username or Password');

                }else{
                    const user = result.rows[0];
                    const token = jwt.sign({userId: user.name, email: user.email}, JWT_SECRET, {expiresIn: '24h'});
                    res.json({token});
                }
            }
        })
    }catch (error){
        console.error('Login Error', error);
    }
})
