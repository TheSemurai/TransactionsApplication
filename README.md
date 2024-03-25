**Application for managing transactions.**<br><br>

**In this application, users can:**<br>
**1. Make requests to retrieve transactions from specific range in their timezone;**<br>
<br>Example:<br>
![image](https://github.com/TheSemurai/TransactionsApplication/assets/58783774/54b1d5cc-59d6-49de-b24d-cab29f32863a)


**2. Make requests to retrieve transactions from range by local time in specific transaction;**
<br>Example:<br> 
![56c0336b-3517-4064-bcd5-71add66264a9](https://github.com/TheSemurai/TransactionsApplication/assets/58783774/dc8c8be0-7a54-4915-aba3-1e82524f2fa0)
<br> <br> <br> 

**3. Make requests to export transactions in Excel (.xlsx);**
In the request you can choose some fields what you wanted to see in Excel file, for example:<br>

`{
  "transactionId": true,
  "name": true,
  "email": false,
  "amount": true,
  "transactionDate": true,
  "clientLocation": false
}`


<br>Response:<br>
![image](https://github.com/TheSemurai/TransactionsApplication/assets/58783774/b6ce2c22-64c3-48fb-9225-e1e5d3d33cca)

<br> <br> <br> 
**4. Make requests to import transactions from file (.csv) at UTC time.**
Some template file for importing:<br>

`transaction_id,name,email,amount,transaction_date,client_location`<br>
`ua1,Holo,holo@a.com,$123,2007-04-01 02:00:05,"50.501595, 30.780512"`<br>
`ua2,Lowrance,lowrance@a.com,$123,2007-04-01 00:00:05,"50.501595, 30.780512"`<br>


<br>Response:<br>

`{
  "success": true,
  "messages": [
    "Transactions was added successfully."
  ]
}`
