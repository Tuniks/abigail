using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList{
    public Quest[] quests = new Quest[2]{
        new Quest(
            "How to Kill One Million Tears",
            // "Mikhail needs my help with his divorce",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.HasTile, "sunflower")
            },
            new QuestStep[2]{
                new QuestStep(
                    1,
                    new QuestCondition(ConditionType.HasTile, "sunflower"),
                    new QuestCondition(ConditionType.HasTile, "cockroach")
                ),
                new QuestStep(
                    2,
                    new QuestCondition(ConditionType.HasTile, "cockroach"),
                    new QuestCondition(ConditionType.Trigger, "sunflower-planted")
                )
            }
        ),
        new Quest(
            "The Bullet",
            // "The mystery of my grandma's bullet necklace",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.HasTile, "pizza")
            },
            new QuestStep[1]{
                new QuestStep(
                    1,
                    new QuestCondition(ConditionType.HasTile, "pizza"),
                    new QuestCondition(ConditionType.ShowTileTo, "pizza", "margarette")
                )
            }
        ),
    };

    public QuestList(){}
}
