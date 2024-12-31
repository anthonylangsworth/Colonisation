**Overview**

Produce a list of potential systems for the upcoming colonization feature for the minor faction *EDA Kunti League* in the game **Elite Dangerous**.

**Use**

1. Download and extract `systemsWithCoordinates.json` from https://www.edsm.net/dump/systemsWithCoordinates.json.gz. Note that this is a large file (12+ GB) at the time of writing.
1. Download and extract `systemsPopulated.json` from https://www.edsm.net/en/nightly-dumps .
1. Compile and run the program, directing its output to a file, e.g. `systemfilter.exe > out.csv`. It may take a few minutes.

Must of this should be moved to configuration or command line arguments but the goal was something simple and quick.

**References**

1. EDSM System APIs: https://www.edsm.net/en/api-system-v1