# 0.13.0

Released on Wednesday, December 25, 2019.

- Updated path's index' calculation in `ComparisonSource` to only include nodes that implement the IParentNode.
- Small change to HtmlDifferenceEngine. It now takes the control and test sources during construction. This makes it clear it is single use. For reusable differ, use `HtmlDiffer` going forward.
- Added interface `IDiffContext` and made `DiffContext` internal.

# 0.13.0-preview-3

Released on Sunday, November 24, 2019.

- Added `Compare(INode controlNode, INode testNode)` to HtmlDifferenceEngine
- Changed existing `Compare` method in HtmlDifferenceEngine to take `IEnumerable<INode>` instead of `INodeList`.

# 0.13.0-preview-2

Released on Sunday, November 3, 2019.

- Fixed error in repository url reported to nuget.

# 0.13.0-preview-1

Released on Sunday, November 3, 2019.

This is the initial preview release of AngleSharp.Diffing. 