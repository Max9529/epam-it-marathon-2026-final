@api @hw8 @l1 @supplemental
Feature: Homework 8 - supplemental catalog checks

  These service checks supplement the UI journeys. The supplied storefront has
  no dish-name search control or route for opening an unknown dish identifier.
  This limitation is recorded in the README instead of inventing a UI flow.

  Rule: Missing catalog entries have a clear outcome

    @negative
    Scenario Outline: A guest requests an unknown entry
      When a guest opens a <item> that does not exist
      Then the guest is told that the <item> could not be found

      Examples: Unknown entries
        | item     |
        | dish     |
        | location |

  Rule: A dish can be found by its name

    Scenario: A guest searches for an individually created dish
      Given the homework restaurant has an individual location
      And the kitchen has added a main course dish
      When a guest searches the menu for that dish by name
      Then that dish appears in the results

  Rule: Unknown credentials are an ordinary refusal

    @negative
    @known-defect
    Scenario: An unknown customer does not cause a server failure
      When someone who never registered tries to sign in
      Then sign-in is refused
