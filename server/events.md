
**host**
emit
```json
{
    "party_name": "<name>'s party",
    "party_max": 6,
    "party_public": false
}
```
return
```json
{
	"successful": true,
	"data": {
		"party_id": "<6 digit hex>"
	}
}
```
```json
{
	"successful": false,
	"data": {
		"error": "couldn't create party"
	}
}
```
---
**disband**
emit
```json
{
    "party_id": "<6 digit hex>"
}
```
return
```json
{
	"successful": true,
	"data": {
		"party_id": "<6 digit hex>"
	}
}
```
```json
{
	"successful": false,
	"data": {
		"error": "you aren't the party host"
	}
}
```
---
**join**
emit
```json
{
    "party_id": "<6 digit hex>"
}
```
return
```json
{
	"successful": true,
	"data": {
    "party_name": "<name>'s party",
    "party_max": 6,
    "party_count": 1,
    "party_public": false,
    "party_host": "<host sid>"
	}
}
```
```json
{
	"successful": false,
	"data": {
		"error": "couldn't find party"
	}
}
```
---
**leave**
emit
```json
{
    "party_id": "<6 digit hex>"
}
```
return
```json
{
	"successful": true,
	"data": {}
}
```
```json
{
	"successful": false,
	"data": {
		"error": "couldn't find party"
	}
}
```
**promote**
emit
```json
{
    "party_id": "<6 digit hex>",
    "new_host": "<6 digit of sid>"
}
```
return
```json
{
	"successful": true,
	"data": {}
}
```
```json
{
	"successful": false,
	"data": {
		"error": "couldn't find user"
	}
}
```
---
---
---
**joined**
emit
```json
{
    "username": "<crab game username>"
}
```
return
```json
{
	"message": "somebody joined the party"
}
```
---
**left**
emit
```json
{
    "username": "<crab game username>"
}
```
return
```json
{
	"message": "somebody left the party"
}
```
---
**disbanded**
emit
```json
{
    "username": "<crab game username>",
    "party_id": "<6 digit hex>"
}
```
return
```json
{
	"message": "somebody disbanded the party"
}
```
---
**message**
emit
```json
{
    "username": "<crab game username>",
    "message": "Hello, world!"
}
```
return
```json
{
	"username": "<crab game username>",
	"message": "Hello, world!"
}
```
---
**promoted**
emit
```json
{
    "old_host": "<crab game username>",
    "new_host": "<crab game username>"
}
```
return
```json
{
	"message": "<old_host> promoted <new_host>"
}
```
