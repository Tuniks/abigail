using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList{
    public Quest[] quests = new Quest[]{
        // QUEST 1
        new Quest(
            "The HOA is Watching You",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.TalkTo, "Margarette"),
            },
            new QuestStep[1]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Margarette")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "sunflower-planted-phenomenon")}                  
                )
            }
        ),
        // QUEST 2
        new Quest(
            "Tanya's Wisdom",
            new QuestCondition[5]{
                new QuestCondition(ConditionType.ShowTileTo, "Butterfly", "Margarette"),
                new QuestCondition(ConditionType.ShowTileTo, "Butterfly", "Ryan 1"),
                new QuestCondition(ConditionType.ShowTileTo, "Butterfly", "Fellini"),
                new QuestCondition(ConditionType.ShowTileTo, "Butterfly", "Gianluca"),
                new QuestCondition(ConditionType.ShowTileTo, "Butterfly", "Timothy")
            },
            new QuestStep[3]{
                new QuestStep(
                   1,
                   new QuestCondition[1] { new QuestCondition(ConditionType.ShowTileTo, "Butterfly", "Margarette") },
                   new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Spider")}        
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Spider")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Gianluca's Pizzeria", "Spider")}                  
                ),
                new QuestStep(
                    3,
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Gianluca's Pizzeria", "Spider")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Tanya")}                  
                )
            },
            1
        ),
         // QUEST 3
        new Quest(
            "Gianluca's Prank War",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.TalkTo, "Gianluca"),

            },
            new QuestStep[3]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Gianluca")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "pizza-plopped-phenomenon")}                  
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Gianluca")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "cockroach-sewer-phenomenon")}
                ),
                new QuestStep(
                    3,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Gianluca")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "ghost-released")}
                )
            }
        ),

        // QUEST 4
        new Quest(
            "Champions of Nostalgia",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.TalkTo, "Timothy"),

            },
            new QuestStep[5]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Timothy")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Ryan 1")}                 
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Ryan 1")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Caterpillar", "Ryan 1")}
                ),
                new QuestStep(
                    3,
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Caterpillar", "Ryan 1")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "goal-scored")}
                ),
                new QuestStep(
                    4,
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "goal-scored")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Soccer Ball", "Timothy")}
                ),
                new QuestStep(
                    5,
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Soccer Ball", "Timothy")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "three-points-scored")}
                )
            }
        ),

         // QUEST 5
        new Quest(
            "The New Best Friend of My Old Best Friend is My Enemy",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.Trigger, "mom-gave-at-end-of-chat"),

            },
            new QuestStep[2]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "mom-gave-at-end-of-chat")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Oz")}                  
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Oz")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.ShowTileTo, "Bunnydeer", "Oz")}//the actual conditions will be bunnydeer and then 2/3 of the following: Stickyhand, ghost, skull
                )
            }
        ),
        // QUEST 6
        new Quest(
            "Brighten the House That Made You",
            new QuestCondition[1]{
                new QuestCondition(ConditionType.Trigger, "mom-gave-at-end-of-chat"),//could be given either at the end of of a dialogue node or after mom is shown butterfly or tuca

            },
            new QuestStep[2]{
                new QuestStep(
                    1,
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "mom-gave-at-end-of-chat")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "sunflower-fixed")}                  
                ),
                new QuestStep(
                    2,
                    new QuestCondition[1] {new QuestCondition(ConditionType.Trigger, "mom-gave-at-end-of-chat")},
                    new QuestCondition[1] {new QuestCondition(ConditionType.TalkTo, "Tuca")}//can be triggered by phenomenon or talking to Tuca for the first time
                )
            }
        )
    };

    public QuestList(){}
}
