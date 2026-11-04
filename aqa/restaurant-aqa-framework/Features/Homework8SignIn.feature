@ui @hw8 @l1
Feature: Homework 8 - customer sign-in outcomes

  Rule: A customer's correct credentials give access

    @smoke
    Scenario: An existing customer signs in
      Given the homework customer has an individual account
      When the homework customer signs in with "correct" credentials
      Then the customer is signed in

  Rule: Invalid credentials do not reveal an account or give access

    @negative
    Scenario Outline: Sign-in is refused with an explanation
      Given the homework customer has an individual account
      When the homework customer signs in with "<credentials>" credentials
      Then the homework storefront refuses sign-in clearly

      Examples: Wrong credentials
        | credentials    |
        | wrong password |
        | unknown email  |
