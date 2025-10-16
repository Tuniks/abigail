using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList{
    public Quest[] quests = new Quest[2]{
        new Quest(
            "How to Kill One Million Tears",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.HasTile, "Sunflower")
            },
            new QuestStep[2]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.HasTile, "Sunflower")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.HasTile, "Cockroach")}
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.HasTile, "Cockroach")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "sunflower-planted")}
                )
            }
        ),
        new Quest(
            "The Bullet",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.HasTile, "Gianluca's Pizzeria")
            },
            new QuestStep[3]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.HasTile, "Gianluca's Pizzeria")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Gianluca's Pizzeria", "Margarette")}
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Gianluca's Pizzeria", "Margarette")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Gianluca")}
                ),
                new QuestStep(
                    3,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Gianluca")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Cockroach", "Gianluca")}
                )
            }
        ),
    };

    public QuestList(){}
}
