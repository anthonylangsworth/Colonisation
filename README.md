**Overview**

Produce a list of potential systems for the upcoming colonization feature for the minor faction *EDA Kunti League* in the game **Elite Dangerous**.

This consists of three command line programs:
1. `Colonisation.Systems`: This produces a list of potential colonisation systems near systems controlled by the specified minor faction. The output is CSV for easy import into Excel or a similar spreadsheet.
1. `Colonisation.Bodies`: This takes the output of `Colonisation.Systems` and looks up details about the bodies on EDSM. The output is in JSON, since it has a complex structure.
1. `Colonisation.Points`: This takes the output of both previous commands and produces a list of systems based on an arbitrary point system to determine the most desirable systems for colonisation. The output is in a CSV.

**Use**

1. Download and extract `systemsWithCoordinates.json` from https://www.edsm.net/dump/systemsWithCoordinates.json.gz into `Colonisation.StarSystems`. Note that this is a large file (12+ GB) at the time of writing.
1. Download and extract `systemsPopulated.json` from https://www.edsm.net/en/nightly-dumps into `Colonisation.StarSystems`.
1. (Optional) Change the `minorFactionName` setting in `Colonisation.StarSystems\applicationSettings.config` to the name of your minor faction. It must match **exactly**.
1. Compile and run `Colonisation.StarSystems`. It may take a few minutes to run. Output is written to `colonisationTargets.csv` by default.
1. Copy `colonisationTargets.csv` into `Colonisation.Bodies` and `Colonisation.Points`.
1. Compile and run `Colonisation.Bodies` to download relevant information about the bodies in these systems.
1. Copy `systemBodies.json` into `Colonisation.Points`.
1. (Future) Compile and run `Colonisation.Points` to output

**References**

1. EDSM System APIs: https://www.edsm.net/en/api-system-v1
1. ED Astro dumps: https://edastro.com/mapcharts/files.html (unused but potentially useful)
1. Inara API: https://inara.cz/elite/inara-api-devguide/ (unused but potentially useful)
