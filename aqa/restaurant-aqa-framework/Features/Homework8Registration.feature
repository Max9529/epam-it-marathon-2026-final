@ui @hw8 @l1
Feature: Homework 8 - customer registration boundaries

  Registration follows the supplied homework contract, including the exact
  boundaries. Form acceptance does not by itself prove an account was created;
  the independent journey below submits the form and signs in afterwards.

  Background:
    Given a visitor is on the sign-up page
    And the homework visitor has filled in valid registration details

  Rule: First and last names have inclusive length boundaries

    @boundary @form
    Scenario Outline: A name has the permitted length
      When the homework visitor uses a "<field>" of length <length>
      Then the homework registration form is "<availability>"

      Examples: First name boundaries
        | field      | length | availability |
        | first name | 1      | unavailable  |
        | first name | 2      | available    |
        | first name | 50     | available    |
        | first name | 51     | unavailable  |

      Examples: Last name boundaries
        | field     | length | availability |
        | last name | 1      | unavailable  |
        | last name | 2      | available    |
        | last name | 50     | available    |
        | last name | 51     | unavailable  |

    @negative
    Scenario Outline: A name explains a disallowed character
      When the homework visitor uses "<value>" as "<field>"
      Then the homework registration form is "unavailable"
      And the homework registration form explains the invalid detail

      Examples: Disallowed names
        | field      | value |
        | first name | An1na |
        | last name  | Te_st |
        | first name | Анна  |
        | last name  | Test! |
        | first name |       |
        | last name  |       |

    Scenario Outline: Permitted punctuation remains usable
      When the homework visitor uses "<value>" as "<field>"
      Then the homework registration form is "available"

      Examples: Valid names
        | field      | value      |
        | first name | Anna-Maria |
        | last name  | O'Neil     |

  Rule: Passwords have inclusive length boundaries

    @boundary @form
    Scenario Outline: A password has the permitted length
      When the homework visitor uses a "password" of length <length>
      Then the homework registration form is "<availability>"

      Examples: Password boundaries
        | length | availability |
        | 7      | unavailable  |
        | 8      | available    |
        | 16     | available    |
        | 17     | unavailable  |

    @negative
    Scenario: Password confirmation must match
      When the homework visitor uses "Different9!" as "password confirmation"
      Then the homework registration form is "unavailable"

  Rule: Email is required, well formed and no longer than 254 characters

    @negative
    Scenario Outline: Invalid email is explained
      When the homework visitor uses "<value>" as "email"
      Then the homework registration form is "unavailable"
      And the homework registration form explains the invalid detail

      Examples: Malformed addresses
        | value            |
        |                  |
        | missing-at.test  |
        | customer@        |
        | @example.com     |

    @boundary @form
    Scenario Outline: The email address reaches its length limit
      When the homework visitor uses an "email" of length <length>
      Then the homework registration form is "<availability>"

      Examples: Email boundary
        | length | availability |
        | 254    | available    |
        | 255    | unavailable  |

  Rule: A submitted registration creates a usable customer account

    @boundary @journey
    Scenario Outline: Accepted boundaries survive registration and sign-in
      When the homework visitor registers with a "<field>" of length <length>
      And the visitor signs in with those details
      Then the customer is signed in
      And the homework signed-in account has only the customer role

      Examples: Accepted inclusive boundaries
        | field      | length |
        | first name | 2      |
        | first name | 50     |
        | last name  | 2      |
        | last name  | 50     |
        | password   | 8      |
        | password   | 16     |

      @known-defect
      Examples: A valid long email is currently rejected by the service
        | field | length |
        | email | 254    |

    @smoke
    Scenario: A newly registered customer signs in through the storefront
      When a visitor signs up with valid details
      And the visitor signs in with those details
      Then the customer is signed in
      And the homework signed-in account has only the customer role
