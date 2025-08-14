# FunGame

To run application over docker use commands->
docker-compose up -d :  to start
docker-compose up down  : to stop

Use baseurl-s: 
http://localhost:5000/api/player
http://localhost:5002/api/score
and endpoints: 
http://localhost:5000/api/player/choices [GET] - get all choices
http://localhost:5000/api/player/choice [GET]   - get random choice
http://localhost:5000/api/player/play [POST] - play the game
http://localhost:5002/api/score/recent [GET] - get 10 recent plays
http://localhost:5064/api/score/reset [DELETE] - Delete history

To note:
Application can be find on the following repo url: 
https://github.com/AjaVelaCode/FunGame.git

I used recoomended random number endpoint.
For the play endpoint, there is a funny facts in the response :) 
There is an endpoint for last 10 plays. 
They can be reset. 
It is dockerized as well. 

Funny fact: I liked yours UI :D 

I enjoyed developing this app. :)