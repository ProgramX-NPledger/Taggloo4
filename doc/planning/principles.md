# Architecture

Principles of development:

* **Portability** of the project, to permit movement between hosts as costs require.
* **Separation** of code from data, to allow respect of license of provided content by not making it available outside of the context of the application.

## Functional Requirements

* The web site must work on a mobile phone.
* Content can be imported and indexed for publication and inclusion in known indexing providing results for translation.
* "Free-form" translations must be available, which can be derived from word adjacency with appropriate indication of confidence.
* Translations can be voted up and down and comments left to support future search queries.

## Non-functional Requirements

* The web site must be responsive, providing results as they become available to give the impression of rapid translation where necessary.
* The web-site must prioritise content according to considered rank, including:
  * votes applied to translation
  * source of translation (dictionary, corpus, computed)
* 