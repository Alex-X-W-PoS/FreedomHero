using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardScript
{
    public string[] trapDescriptions = {
        "As you were walking through, you notice a contraption that makes a rock fall your way, unable to avoid it, you get hurt by it.\n\nYou receive <b>@amount</b> DMG",
        "Walking without looking around might be dangerous. You stepped on a spike trap and it hurts.\n\nYou receive <b>@amount</b> DMG"
    };

    public string[] healingDescriptions = {
        "As you traverse the place, you feel the calm emanating from the surrounding area and you decide to take a short rest.\n\nYou recover <b>@amount</b> HP",
        "Walking through, you find a tree full of apples, you decide to take one and eat it, as you're getting hungry.\n\nYou recover <b>@amount</b> HP"
    };

    public string[] encounterDescriptions = {
        "As you're walking through, you heard some rambling sound. Looking at the sound origin, you find some enemies coming your way.\n\nEnemy Encounter.",
        "As you think you're out of danger, you hear a weird groan. It's an enemy! And it's coming your way!\n\nEnemy Encounter."
    };

    public string[] itemGiveDescription = {
        "You help a man you find in problem, and the man feel grateful to you, so he decides to give you something.\n\nReceive 1 <b>@itemName</b>.",
        "Looking around in the area nearby, you find something shiny, you go to it and it seems to be an item! You take it.\n\nReceive 1 <b>@itemName</b>."
    };

    public string defaultItemScreenText = "You got 1 @ItemName!";

    public EventData GenerateRandomEvent(DataManager dataManager) {
        EventData eventToReturn;
        float typeOfEvent = Random.Range(0f,100f);
        string eventType;
        if(typeOfEvent >= 85f) {
            eventType = "item";
        }
        else if (typeOfEvent >= 70f) {
            eventType = "encounter";
        }
        else if (typeOfEvent >= 35) {
            eventType = "heal";
        }
        else {
            eventType = "trap";
        }
        int indexText = Random.Range(0, 2);

        string textDescription;
        string action;
        switch (eventType)
        {
            case "item":
                textDescription = itemGiveDescription[indexText];
                action = "add";
                break;
            case "encounter":
                textDescription = encounterDescriptions[indexText];
                action = "none";
                break;
            case "heal":
                textDescription = healingDescriptions[indexText];
                action = "add";
                break;
            default:
                textDescription = trapDescriptions[indexText];
                action = "substract";
                break;
        }
        
        string subject;
        int amount = 0;
        string realText;
        int itemCode = 0;
        if(eventType == "heal" || eventType == "trap") {
            int randomAmount = Random.Range(1, 7);
            amount = randomAmount * 5;
            subject = amount.ToString();
            realText = this.ChangeVariable("@amount",subject,textDescription);
        }
        else {  
            //////////FILL FOR ITEM OR ENEMY ENCOUNTER
            if (eventType == "item") {
                int probability = Random.Range(1,101);
                if(probability >85) {
                    itemCode = 6;
                }
                else if (probability >75) {
                    itemCode = 5;
                }
                else if (probability >60) {
                    itemCode = 4;
                }
                else if (probability >45) {
                    itemCode = 3;
                }
                else if (probability >40) {
                    itemCode = 2;
                }
                else if (probability >25) {
                    itemCode = 1;
                }
                string itemName = dataManager.player.itemBag[itemCode].item.name;
                realText = this.ChangeVariable("@itemName",itemName,textDescription);
            }
            else {
                realText = textDescription;
            }   
        }

        eventToReturn = new EventData(eventType,realText,action,amount,itemCode);

        return eventToReturn;
    }

    public string ReturnItemPanelText(ItemData item) {
        string defaultText = this.defaultItemScreenText;
        string textToReturn = this.ChangeVariable("@ItemName",item.name,defaultText);
        return textToReturn;
    }

    public string ReturnDefaultItemPanelText() {
        string defaultText = this.defaultItemScreenText;
        return defaultText;
    }

    public string ChangeVariable(string variable, string value, string text) {
        string newText =text.Replace(variable,value);
        return newText;
    }


}
