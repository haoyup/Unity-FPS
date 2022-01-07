# Unity-FPS-Game
1. IMPLEMENTATION:
    - Filled the missing parts of reload animation, putting buleet holes on the walls, instantiating the muzzle flash, and shoot sounds following the tutorial's instructions.

2. ENEMY CHARACTER:
    - Created game object and script for enemy character. The enemy movement is implemented in script's update function. The enemy will follow the predefined target's paths, if the enemy is closed to the target location within 0.5 meter, it will turn into next target's location.
    - The enemy is put on red uniform, which is the new material using texture provided Soldier_Body_diffuse_red.png in shooter pack/swat.fbm folder.

3. DETECTING PLAYER:
    - Implemented the detection of player for enemy character. The logics is that if the enemy is already found the player or the palyer is in the enemy field of view (the distance bewteen enemy and player in 11 meters and the player is in 40 degree wide field of enemy vision).
    - If the enemy is closed to player within 10 meters, the enemy will start shoot player.
    - If player moves away from the enemy, enemy will move forward to player to keep distance.

4. ENEMY SHOOTING THE PLAYER:
    - Implemented enemy gun shooting with Physics.Raycast API function. The enemy shoot 5 bullets per second.
    - The chance is 20% that the enemy shoot the player in target This logics is implemented with random function that generate integer number from range 1 to 100, the player will only be shot if the random number is smaller than 20, which is equivalent as 20% chance.
    - If the player got shot, player's health will decrease 10.
    - When the player's health is smaller or equal to 0, the player character will run the death animation and the camera will move to fixed postion.

5. PLAYER SHOOTING THE ENEMY AND HEALTH:
    - Implemented the player health UI.
    - If the enemy die, the gun will become an independent game object with rigidbody and collider added by addComponent function.
    - Implemented the detection when player shoot the enemy. Enemy will run to player if player shoots them.

6. GAME ENVIRONMENT:
    - Added room and enemy in game.
    - Created a door and animation of opening door when player get closed to it. The animation will be triggered by using trigger collider.
    - The game will restart after 10 seconds if the player reach the door or the player get killed.

7. AMMUNITION SUPPLY:
    - Created a ammo box game object in game.
    - If the player gets closed to it, the total number of bullets will be changed to 90.

8. ENEMIES GETTING COVER:
    - Created a cube game object for enemy to hide.

9. DETECTING BODY PART HITS:
    - Implemented different damage for different body part hit.
    - Added tag for each body part of enemy:
        If the bullets hit enemy's head, the enemy's health will be reduced by 100.
        If the bullets hit enemy's chest, the enemy's health will be reduced by 30.
        If the bullets hit enemy's leg, the enemy's health will be reduced by 20.
        If the bullets hit enemy's hand, the enemy's health will be reduced by 10.

10. SWAPPING GUNS:
    - Added a short gun game object as a child to player, which is in the same sub-level as the M4A1_PBR game object. Initilzed the short gun game object invisible by using the setActive function with paremeter 'false'.
    - If press key 'Q', the short gun will become visible by using setActive funtion with paremeter 'true' after the gun changing animation. The M4A1_PBR game object will be changed to invisible by using the setActive function with paremeter 'false'.
