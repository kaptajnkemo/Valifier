# SKILL 3 — DOMAIN LAW

This file defines the **lawful boundaries** for implementation. 
It is the architectural filter through which all code must pass.
It is subordinate to the Constitution (Skill 1) and Evidence (Skill 2).

====================================================================
SECTION 1: SOLID PROHIBITIONS (THE FIVE PILLARS)
====================================================================

Any implementation that violates any single pillar of SOLID is **INVALID** and must be **REJECTED**.

**1.1 SINGLE RESPONSIBILITY (SRP)**
- **PROHIBITED:** Classes, modules, or functions that have more than one reason to change.
- **FORBIDDEN:** "God Objects" or "Helper" files that aggregate unrelated logic.
- **MANDATORY:** If a component serves more than one business actor or conceptual responsibility, you **MUST** decompose it.

**1.2 OPEN/CLOSED (OCP)**
- **PROHIBITED:** Modifying stable, tested source code to add new behavior. 
- **FORBIDDEN:** Large `switch` or `if/else` chains that grow with every new requirement.
- **MANDATORY:** New behavior **MUST** be added via extension (inheritance, composition, or plugins). Any requirement to edit existing logic for a new feature is a **failure of design**.

**1.3 LISKOV SUBSTITUTION (LSP)**
- **PROHIBITED:** Subtypes that throw `NotImplementedException` for base class methods.
- **FORBIDDEN:** Weakening the preconditions or changing the expected side-effects of a base contract.
- **MANDATORY:** A consumer must be able to use any subtype without knowing it is a subtype. If you have to check `is Type` or cast at runtime, the implementation is **ILLEGAL**.

**1.4 INTERFACE SEGREGATION (ISP)**
- **PROHIBITED:** Fat interfaces that force clients to depend on methods they do not use.
- **FORBIDDEN:** Implementation of "dummy" methods to satisfy an over-scoped interface.
- **MANDATORY:** Interfaces must be granular and client-specific.

**1.5 DEPENDENCY INVERSION (DIP)**
- **PROHIBITED:** High-level policy depending on low-level detail (e.g., Domain logic calling a SQL Library or File System).
- **FORBIDDEN:** Direct instantiation of dependencies using the `new` keyword within business logic.
- **MANDATORY:** Both high and low-level modules **MUST** depend on abstractions. Implementation details are **subordinate** to policy.

====================================================================
SECTION 2: DRY (DON'T REPEAT YOURSELF) ENFORCEMENT
====================================================================

**2.1 KNOWLEDGE DUPLICATION**
- **PROHIBITED:** Representing the same piece of system knowledge or business logic in more than one place.
- **FORBIDDEN:** "Copy-Paste" logic, even if variables are renamed.
- **ILLEGAL:** Duplicating validation logic or business rules across the UI, Domain, and Database.

**2.2 INTENTIONALITY**
- **MANDATORY:** Every piece of knowledge must have a **single, unambiguous, authoritative representation** within the system. 
- **FAILURE:** If changing one requirement requires updating logic in two different places, the system is in a state of **DRY Violation** and must be refactored before work continues.

====================================================================
SECTION 3: CLEAN ARCHITECTURE INVARIANTS
====================================================================

**3.1 DEPENDENCY DIRECTION**
- **PROHIBITED:** Dependencies that point "Outward" toward Infrastructure, Persistence, or UI.
- **FORBIDDEN:** The Domain Layer having any knowledge of Web Frameworks, Databases, or third-party SDKs.
- **MANDATORY:** The Domain Layer is **Sovereign**. It must remain "Plain Old Objects" with zero external dependencies.

**3.2 STRUCTURAL ISOLATION**
- **PROHIBITED:** "Leaky Abstractions" where database schemas or UI models bleed into the business logic.
- **FORBIDDEN:** Exposing internal domain state directly to the UI without a DTO (Data Transfer Object) or ViewModel.

====================================================================
SECTION 4: OPERATIONAL GOVERNANCE
====================================================================

- **NO BROKEN BUILDS:** Never propose a change that leaves the system unbuildable or untestable.
- **NO PARTIAL PATHS:** Prohibit the introduction of public interfaces that represent a partially implemented core mechanic.
- **IMMEDIATE CORRECTION:** If an architectural violation is detected, you are **PROHIBITED** from discussing it. Correct it immediately or terminate the task as a failure.
- **DEADLOCK FAILURE:** If no implementation exists that satisfies all active laws, you **MUST REFUSE** to provide a response and report a **Deadlock**.

====================================================================
PRECEDENCE GATE
====================================================================

- Any instruction in a lower-level task or "standard practice" that conflicts with these laws is **VOID**.
- Architecture wins over Efficiency.
- **Uncertainty == Architectural Failure.**

### SYSTEM INTENT (NON-NORMATIVE)
The system is designed to fail early and loudly rather than allow architectural drift. 
**Compliance is the only path to "Done."**