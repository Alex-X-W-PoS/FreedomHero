using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneratorScript
{
    public Vector3 WorldToGrid (Vector3 t, bool isPlayer) {
        Vector3 vectorToReturn;
        if(isPlayer) {
            vectorToReturn = new Vector3(-((2*t.y + t.x)/4),((t.x - 2*t.y)/4),t.z);
        }
        else {
            vectorToReturn = new Vector3(-((2*t.y + t.x)/4),((t.x - 2*t.y)/4),((t.x - 2*t.y)/4));
        }
        return vectorToReturn;
    }

    public Vector3 GridToWorld (Vector3 t, bool isPlayer) {
        Vector3 vectorToReturn;
        if(isPlayer) {
            vectorToReturn = new Vector3((t.x*-2 - t.y),(t.x*2 - t.y),t.z);
        }
        else {
            vectorToReturn = new Vector3((t.x*-2 - t.y),(t.x*2 - t.y),(t.z*-1));
        }
        return vectorToReturn;
    }

    public BlockData generateRandomBlock (int newX, int newY, string direction, int blocksInMap, int greaterEnemiesOnMap,int counterNonEventBlocks,int numberOfItemsOnMap, int enemiesOnMap) {
        BlockData blockToReturn;
        bool up = false;
        bool down = false;
        bool right = false;
        bool left = false;

        string type = "normal";
        
        switch (direction)
        {
            case "up":
                down = true;
                if (newX == 0) {
                    up = false;
                }
                else {
                    up = Random.Range(0f,1f) >= 0.5f;
                }
                if (newY == 0) {
                    left = false;
                    right = Random.Range(0f,1f) >= 0.4f;
                }
                else if (newY == 8){
                    right = false;
                    left = Random.Range(0f,1f) >= 0.4f;
                }
                else {
                    right = Random.Range(0f,1f) >= 0.4f;
                    left = Random.Range(0f,1f) >= 0.4f;
                }
                break;
            case "down":
                up = true;
                if(newX == 8) {
                    down = false;
                }
                else {
                    down = Random.Range(0f,1f) >= 0.5f;
                }
                if (newY == 0) {
                    left = false;
                    right = Random.Range(0f,1f) >= 0.4f;
                }
                else if (newY == 8){
                    right = false;
                    left = Random.Range(0f,1f) >= 0.4f;
                }
                else {
                    right = Random.Range(0f,1f) >= 0.4f;
                    left = Random.Range(0f,1f) >= 0.4f;
                }
                break;
            case "right":
                left = true;
                if (newY == 8) {
                    right = false;
                }
                else {
                    right = Random.Range(0f,1f) >= 0.5f;
                }
                if (newX == 0) {
                    up = false;
                    down = Random.Range(0f,1f) >= 0.4f;
                }
                else if (newX == 8){
                    down = false;
                    up = Random.Range(0f,1f) >= 0.4f;
                }
                else {
                    up = Random.Range(0f,1f) >= 0.4f;
                    down = Random.Range(0f,1f) >= 0.4f;
                }
                break;
            case "left":
                right = true;
                if (newY == 0) {
                    left = false;
                }
                else {
                    left = Random.Range(0f,1f) >= 0.5f;
                }
                if (newX == 0) {
                    up = false;
                    down = Random.Range(0f,1f) >= 0.4f;
                }
                else if (newX == 8){
                    down = false;
                    up = Random.Range(0f,1f) >= 0.4f;
                }
                else {
                    up = Random.Range(0f,1f) >= 0.4f;
                    down = Random.Range(0f,1f) >= 0.4f;
                }
                break;
            default:
                break;
        }

        float isEventPanel = Random.Range(0f,100f);

        
        if (isEventPanel >= 70f) { //is an event panel
            float eventKind = Random.Range(0f,100f);
            if (eventKind >= 85f) {
                type = "greater_enemy";
            }
            else if (eventKind >= 65f) {
                type = "store";
            }
            else if (eventKind >= 40f) {
                type = "enemy";
            }
            else {
                type = "event";
            }
        }
        else {
            if (counterNonEventBlocks > 2){ //is also an event panel
                float eventKind = Random.Range(0f,100f);
                if (eventKind >= 85f) {
                    type = "greater_enemy";
                }
                else if (eventKind >= 65f) {
                    type = "store";
                }
                else if (eventKind >= 40f) {
                    type = "enemy";
                }
                else {
                    type = "event";
                }
            }
            else {
                type = "normal";
            }
        }

        if (blocksInMap >= 22 && greaterEnemiesOnMap < 2) {
            type = "greater_enemy";
        }

        if (numberOfItemsOnMap == 2 && type == "store") {
            type = "event";
        }

        if (greaterEnemiesOnMap == 2 && type == "greater_enemy") {
            type = "event";
        }

        if ((blocksInMap == 6 || blocksInMap == 12 || blocksInMap == 18) && enemiesOnMap < 3) {
            type = "enemy";
        }

        if (enemiesOnMap == 4 && type == "enemy") {
            type = "event";
        }

        if (blocksInMap >= 24) {
            type = "exit";
        }
        /*if (type == "greater_enemy") {
            type = "enemy";
        }*/
        int blockImage = this.generateBlockImageNumber(up,down,right,left,type);

        blockToReturn = new BlockData(true,blockImage,up,down,right,left,type,0);

        return blockToReturn;
    }


    public void generateRandomSeed () {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public int generateBlockImageNumber (bool up, bool down, bool right, bool left, string type) {
        int blockImageNumber;
        if (up && down && right && left) {
            blockImageNumber = 15;
        }
        else {
            if(up) {
                if (down) {
                    if (right){
                        blockImageNumber = 1;
                    }
                    else {
                        if (left) {
                            blockImageNumber = 2;
                        }
                        else {
                            blockImageNumber = 3;
                        }
                    }
                }
                else {
                    if (right) {
                        if(left) {
                            blockImageNumber = 4;
                        }
                        else {
                            blockImageNumber = 5;
                        }
                    }
                    else {
                        if(left) {
                            blockImageNumber = 6;
                        }
                        else {
                            blockImageNumber = 7;
                        }
                    }
                }
            }   
            else {
                if (down) {
                    if (right) {
                        if (left) {
                            blockImageNumber = 8;
                        }
                        else {
                            blockImageNumber = 9;
                        }
                    }
                    else {
                        if (left) {
                            blockImageNumber = 10;
                        }
                        else {
                            blockImageNumber = 11;
                        }
                    }
                }
                else {
                    if (right) {
                        if (left) {
                            blockImageNumber = 12;
                        }
                        else {
                            blockImageNumber = 13;
                        }
                    }
                    else {
                        blockImageNumber = 14;
                    }
                }
            }
        }

        int multiplier = 0;

        switch(type) {
            case "normal":
                multiplier = 0;
                break;
            case "event":
                multiplier = 1;
                break;
            case "enemy":
                multiplier = 2;
                break;
            case "greater_enemy":
                multiplier = 3;
                break;
            case "store":
                multiplier = 5;
                break;
            case "exit":
                multiplier = 6;
                break;
            default:
                break;
        }

        blockImageNumber = blockImageNumber + 15*(multiplier);

        return blockImageNumber;
    }
}
