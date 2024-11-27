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
app.post('/register', async (req, res) => {
    try {
        const { name, email, password } = req.body;

        // Check if all required fields are present
        if (!email || !name || !password) {
            return res.status(400).send('Missing one or more fields');
        }

        // Query to check for existing users with the same name or email
        const checkUserQuery = `
            SELECT * 
            FROM users 
            WHERE name = $1 OR email = $2
        `;

        const checkResult = await client.query(checkUserQuery, [name, email]);

        if (checkResult.rows.length > 0) {
            return res.status(409).send('User with the same email or username already exists');
        }

        // Insert the new user if no duplicates were found
        const insertQuery = `
            INSERT INTO users (name, email, password) 
            VALUES ($1, $2, $3)
        `;

        await client.query(insertQuery, [name, email, password]);

        res.status(201).send('Registration Successful');
    } catch (error) {
        console.error('Register Error:', error);
        res.status(500).json({ error: 'Server error' });
    }
});

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
                    const token = jwt.sign({userId: user.name, email: user.email}, JWT_SECRET, {expiresIn: '30d'});
                    res.json({token});
                }
            }
        })
    }catch (error){
        console.error('Login Error', error);
    }
})

app.get('/leaderboard', authenticateToken, (req, res) => {
    const query = `
        SELECT id, name, kills, deaths, 
               CASE WHEN deaths = 0 THEN kills ELSE CAST(kills AS FLOAT) / deaths END AS kd_ratio 
        FROM users 
        ORDER BY kd_ratio DESC 
        LIMIT 10;
    `;

    client.query(query, (err, result) => {
        if (!err) {
            res.json(result.rows);
        } else {
            console.log(err.message);
            res.status(500).send('Failed to fetch leaderboard');
        }
    });
});


/*
app.put('/users/:id/score', authenticateToken, (req, res) => {
    const { kills, deaths } = req.body;
    const userId = req.params.id;

    // Input validation
    if (typeof kills !== 'number' || typeof deaths !== 'number') {
        return res.status(400).send('Kills and deaths must be numbers');
    }

    const updateQuery = `
        UPDATE users 
        SET kills = $1, deaths = $2 
        WHERE id = $3
        RETURNING name, kills, deaths;
    `;

    client.query(updateQuery, [kills, deaths, userId], (err, result) => {
        if (!err) {
            if (result.rows.length > 0) {
                res.json(result.rows[0]);  // Return updated stats
            } else {
                res.status(404).send('User not found');
            }
        } else {
            console.log(err.message);
            res.status(500).send('Failed to update score');
        }
    });
});

app.put('/users/updateKillDeath/:email', async (req, res) => {
    const { kills, deaths } = req.body;
    const { email } = req.params;
    
    try {
        const user = await User.findOne({ email: email });
        if (user) {
            user.kills = kills;
            user.deaths = deaths;
            await user.save();
            return res.status(200).json({ message: "Stats updated successfully" });
        } else {
            return res.status(404).json({ message: "User not found" });
        }
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: "Error updating stats" });
    }
});*/
