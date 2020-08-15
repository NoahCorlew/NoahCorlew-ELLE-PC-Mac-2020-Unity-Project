/*Sorry I stopped updating this page
 * 
 * 
 * 
 * 
 * Hand Physics Folder: This is where the scripts related to moving the physical cards around is.
 * At its core, the hand system works off a set of nodes, where each physical card has a node that it is pathing towards at all times.
 * 
 * HandPhysicsManager.cs: Manages the spawning and moving of hand nodes, spawning of cards, and moving multiple cards at once.
 * 
 * HandNodeID.cs: Script attached to the hand nodes. Used when a physical card needs to find the ID of the node closest to it.
 * 
 * CardPhysics.cs: Manages the movement of the physical card left and right on the nodes, mouse moving the cards, viewing the card that you are hovering over, moving cards to other hand nodes
 *      or question nodes. This script can tell HandPhysicsManager to move multiple cards at once.
 * 
 * Cards do not have any identifying info on them yet like the answer phrase. (This will be added when the backend is hooked up and with a cardProps.cs script that will be on the physical card).  
 *      
 * When the game is started, HandPhysicsManager spawns 11 hand nodes and places them at a location offscreen and disables them. 
 * 
 * Cards are drawn through HandPhysicsManager. When a card is drawn, the number of nodes is increased by one and moveNodes() is called to move the hand nodes to reasonable positions for the
 *      number of cards in the players hand. findCards() is run to update the cardGOs list of card game objects, which is frequently used throughout the game. T
 * 
 * An important insight for the card moving system is that cards will only ever move one position left or right at a time. When the player is grabbing a card, and the card is no longer near the node
 *      that it was originally on, the new node that the card is closest to is found and all cards left or right of that new closest node are move one position left or right. This works because a 
 *      node to the left or right of all the cards to be moved has just been made vacant by the player moving the card that they are grabbing with their mouse. Once this has happened, this process is
 *      repeated, but the "node that it [the card being moved] was originally on" is now the node that was made vacant by the other cards moving left and right.
 *      
 * When the player releases the mouse button and was holding a card, the card searches for the closest question drop node. If it is close enough, the card is deleted, all the cards to the right of the 
 *      node that the now deleted card move left, and the leftmost node is removed by decrementing the number of nodes stored in HandPhysicsManager and running moveNodes()
 * 
 * 
 * 
 * QuestionManager creates a question, then calls createQuestionDropArea(question) in CardDropManager. CardDropManager then calls createQuestionDropArea(question) to create a new question drop area
 *      for answering questions.
 * 
 * 
 */
