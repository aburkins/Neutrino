# Neutrino

After downloading this project, you'll need to change the path pointed to in line 108 of ReadData.cs to an absolute path to the file called "tour.txt" (named so because it was used for tours of DiVE, as it contains many interesting events) on your local file system.


CONTROLS:

Move the camera in space with wasd. Use the mouse to change direction.
Press f to move forward to the next event. Press r to return to the previous event.

About the project:
This project was undertaken in the summer of 2015 in collaboration with the Duke Immersive Virtual Environment and the Duke Neutrino Group. The goal was to redesign an existing 2D application for viewing neutrino detector data into a virtual reality experience. Elements of this project are adapted from Elizabeth Izatt, who first worked on the Super-K Duke Immersive Virtual Environment experience, an application developed in C++ and open-source Syzygy virtual reality software, before graduating and entering UC Berkley. The updated project is designed to be adaptable between an Oculus Rift or the Duke Immersive Virtual Environment, but here it has been adapted as a standalone desktop application. 

 The cylinder is a representation of the Super-Kamiokande water Cherenkov detector. There are inner and outer detectors; the outer detectors are denoted by a box around them. The data are measurements taken from both simulated and real collisions measured by the detector, located in Gifu, Japan. From Wikipedia, "A neutrino interaction with the electrons or nuclei of water can produce a charged particle that moves faster than the speed of light in water (not to be confused with exceeding the speed of light in a vacuum). This creates a cone of light known as Cherenkov radiation, which is the optical equivalent to a sonic boom. The Cherenkov light is projected as a ring on the wall of the detector and recorded by the PMTs. Using the timing and charge information recorded by each PMT, the interaction vertex, ring direction and flavor of the incoming neutrino is determined." The point where the yellow lines converge represents the location of the simulated collision, but the vertex may also be estimated given the ring. The photomultiplier tubes are colored based on their charge, with brighter tubes absorbing more light.
 
 The hardest challenges of this project for me was to understand and implement the geometry that governs much of what is shown, including the placement of the photomultiplier tubes, the detector cylinder, and the Cherenkov cones. The data file contains some data about each tube such as its x-y-z position in space and its charge, and the mapping of charges to colors was provided to me by Liz Izatt and Dr. Kate Scholberg. However, the rotation of each tube or "dot" as they are referred to in the code is depenedent upon calculating the rotational direction orthogonal to the outside of a cylinder. Likewise, Cherenkov cones are determined by an algorithm that involves calculating and drawing a cone given a vertex and a circle on the cylinder. Challenging as it was, working on this project was both enjoyable and fulfilling. 

Read more about Super-K and the Duke Neutrino Group at http://neutrino.phy.duke.edu/

