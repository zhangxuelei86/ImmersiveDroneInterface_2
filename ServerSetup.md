# Server Setup

Look at Radiation Visualization's Hololens Testing.md

## Server Installation

1. [Install ROS](http://wiki.ros.org/melodic/Installation/Ubuntu).
    - Configure repositories, sources and keys.
    - `sudo apt install ros-melodic-desktop`
    - `echo "source /opt/ros/melodic/setup.bash" >> ~/.bashrc`
    - `source ~/.bashrc`
    - `sudo apt install python-rosdep python-rosinstall python-rosinstall-generator python-wstool build-essential`
    - `rosdep init`
    - `rosdep update`
1. Install ROSBridge-suite.
    - `sudo apt install ros-melodic-rosbridge-suite`
1. Install custom messages. Remember to add them to your .bashrc file.
    - VoxBlox Messages: https://voxblox.readthedocs.io/en/latest/pages/Installation.html
    - [PCFace Messages](#setting-up-pcface-messages)
    - `echo "source [path to catkin_ws/]devel/setup.bash" >> ~/.bashrc`

## Setting up PCFace messages

1. Create a Catkin workspace if you have not already
1. `cd src/`
1. Create a package called `rntools`
    1. `catkin_create_pkg rntools std_msgs rospy roscpp`
1. `cd rntools`
1. `mkdir msg`
1. Place `PCFace.msg` inside the `msg` directory
1. Follow the instructions in [this tutorial](http://wiki.ros.org/ROS/Tutorials/CreatingMsgAndSrv#Creating_a_msg) to modify `package.xml` and `CMakeLists.txt`
1. Return to your catkin workspace root directory, (ex. `cd ~/catkin_ws`)
1. `catkin build`
1. `source devel/setup.bash`
1. Verify that everything works with `rosmsg show PCFace`. You should see a listing under the `rntools` package.
