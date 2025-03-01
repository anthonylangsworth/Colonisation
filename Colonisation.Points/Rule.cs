using Colonisation.Common;

namespace Colonisation.Points;

// Point guidance:
// Rules give 1 to 5 points, generally:
// 1: Something cosmetic, e.g. multiple stars
// 3: Something useful 
// 5: Something compelling, e.g. a landable planet for potential settlements, minable rings

abstract class Rule
{
    // The return type is the point count followed by a short description. 
    public abstract (int points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies);
}

class LandableWorlds: Rule
{
    public override (int points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Body> landableWorlds = bodies.bodies.Where(body => body.isLandable);
        return landableWorlds.Any()
            ? (5, $"Contains landable worlds: {string.Join(", ", landableWorlds.Select(bodyInfo => bodyInfo.name))}")
            : (0, "");
    }
}

class MultipleStars : Rule
{
    public override (int points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Body> stars = bodies.bodies.Where(body => body.type == "Star");
        bool isDistant = bodies.bodies.Any(body => body.type == "Star" && body.distanceToArrival > 50_000);
        if(stars.Any() && isDistant)
        {
            return (1, $"Contains multiple but some distant stars: {string.Join(", ", stars.Select(bodyInfo => bodyInfo.name))}");
        }
        else if(stars.Any())
        {
            return (2, $"Contains multiple stars: {string.Join(", ", stars.Select(bodyInfo => bodyInfo.name))}");
        }
        else
        {
            return (0, "");
        }
    }
}

class Rings : Rule
{
    public override (int points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Body> ringedWorlds = bodies.bodies.Where(body => body.rings.Any());
        return ringedWorlds.Any()
            ? (5, $"Contains ringed worlds: {string.Join(", ", ringedWorlds.Select(bodyInfo => bodyInfo.name))}")
            : (0, "");
    }
}

class TerraformableWorlds : Rule
{
    public override (int points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Body> terraformableWorlds = bodies.bodies.Where(body => !string.IsNullOrWhiteSpace(body.terraformingState) && body.terraformingState != "Not terraformable");
        return terraformableWorlds.Any()
            ? (20, $"Contains terraformable worlds: {string.Join(", ", terraformableWorlds.Select(bodyInfo => bodyInfo.name))}")
            : (0, "");
    }
}