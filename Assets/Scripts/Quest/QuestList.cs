using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList{
    public Quest[] quests = new Quest[2]{
        new Quest(
            "How to Kill One Million Tears",
            "Mikhail needs my help with his divorce", 
            new QuestStep[2]{
                new QuestStep(
                    "01",
                    new QuestCondition(ConditionType.HasTile, "sunflower"),
                    new QuestCondition(ConditionType.HasTile, "cockroach"),
                    "Bugs... she always loved the ugliest of us",
                    "Was I to her what she was to me?"
                ),
                new QuestStep(
                    "02",
                    new QuestCondition(ConditionType.HasTile, "cockroach"),
                    new QuestCondition(ConditionType.Trigger, "sunflower-planted"),
                    "Always looking for a spot in the sun",
                    "I wasn't"
                )
            }
        ),
        new Quest(
            "The Bullet",
            "The mystery of my grandma's bullet necklace",
            new QuestStep[1]{
                new QuestStep(
                    "01",
                    new QuestCondition(ConditionType.HasTile, "pizza"),
                    new QuestCondition(ConditionType.ShowTileTo, "pizza", "margarette"),
                    "Tomato sauce red, thicker but just as sweet as...",
                    "Of course Margarette would know"
                )
            }
        ),
    };

    public QuestList(){}
}
