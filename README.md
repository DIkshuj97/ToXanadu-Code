To Xanadu:

Xanadu is a fictional city that was famously described in the poem "Kubla Khan" by Samuel Taylor Coleridge. In the poem, Xanadu is the magnificent palace of Kubla Khan, the ruler of a vast and exotic empire. 
A Non linear adaptation of Invisible cities by Italo Calvino. Its a team project during 2nd Semester of my masters where i worked as the programmer.

I have created SaveSystem for this game and i have used JSON file to store the data. For this i have created a seperate script which will create a new save file in beginning and update the file as the game progress.
In addition, this game contain two game mechanics. Firstly, a memory game where player has to match same cards from pile of cards. In this game mechanics, i am randomizing the card at beginning of level and then changing the sprite and also after completing the
level. Save system will record if player won the level or not. Also, if player wins, there is a possibility of winning the power card which will also stored in save file. 

Second mechanics is also a card game mechanics, where player will play against AI and both can use power card. For that, i programmed AI to use card depending upon the previous move by player which is stored in different AIdata file. If AI is losing, 
it can also use best power cards for the game. 

Also Game contains a map, which shows how many city has unlocked and locked. This data is also stored in JSON file. It will always refresh the data whenever map scene is loaded.

You can play this game at: https://housein.itch.io/to-xanadu
