# CSharp.ArchiecturalDecisionRecords
C# Architectural Decision Records


Allowing C# code to be annotated with [Architectural Decision Records](https://adr.github.io) that conform to a defined schema.
Architectural Decision Records are validated for completeness using a Roslyn analyser.
This work is inspired by existing solutions for Embedded ADRs in source code such as https://github.com/adr/e-adr for Java


The idea is for these records to:
- Be extracted into a popular format for documentation purposes on a regular basis
- Serve as a living documentation in code of significant architectural decisions

## Annotation Format
```
///<summary>
/// Your usual summary comments go here
///</summary
///<adr>
///<useCase></useCase>
///<concernBeingAddessed></concernBeingAddessed>
///<agreedApproach></agreedApproach>
///<acceptedDownside></acceptedDownside>
///<otherOptionConsidered></otherOptionConsidered>
///<otherOptionConsidered></otherOptionConsidered>
///<engineerSignoff></engineerSignoff>
///</adr>
public class MyClass()
{
}
```

## Generated Text
### MyClass Architectural Decision Record
```
In the context of <useCase>,
facing concern <concernBeingAddressed>,
we decided for <agreedApproach>,
having considered <otherOptionConsidered>.
We accept <acceptedDownside> as a trade-off.
Signed off by: <engineerSignoff>
```
