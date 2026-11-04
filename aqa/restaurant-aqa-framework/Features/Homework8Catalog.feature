@ui @hw8 @l1
Feature: Homework 8 - browsing the restaurant catalog

  A guest sees real catalog data. The browser responses are observed, never mocked.
  Each scenario owns its sample location and dishes and removes them afterwards.

  Background:
    Given the homework restaurant has dishes in every menu category

  Rule: Category filters only show the requested category

    Scenario Outline: A guest selects a category
      When the homework guest selects "<category>" on the menu
      Then the homework menu shows only "<category>" dishes

      Examples: All categories
        | category     |
        | Appetizers   |
        | Main Courses |
        | Desserts     |

    Scenario: A guest can return to the complete catalog
      When the homework guest selects "Appetizers" on the menu
      And the homework guest clears that category filter
      Then the homework menu shows the complete catalog

  Rule: The guest chooses how the menu is ordered

    Scenario Outline: A guest sorts by price
      When the homework guest sorts the menu by "<sorting>"
      Then the homework menu prices are "<direction>"

      Examples: Both price directions
        | sorting          | direction  |
        | Price Ascending  | ascending  |
        | Price Descending | descending |

    Scenario Outline: Popularity sorting preserves the service order
      When the homework guest sorts the menu by "<sorting>"
      Then the homework menu preserves the returned popularity order

      Examples: Both popularity directions
        | sorting               |
        | Popularity Ascending  |
        | Popularity Descending |

  Rule: A guest can inspect a dish

    Scenario: The selected dish opens with its details
      When the homework guest opens a sample dish on the menu
      Then the homework dish details match the selected dish

  Rule: The home page offers popular dishes and locations

    Scenario: A guest sees the restaurant on the home page
      When the homework guest opens the restaurant home page
      Then the homework home page shows popular dishes and locations
