// Copyright (c) FIRST and other WPILib contributors.
// Open Source Software; you can modify and/or share it under the terms of
// the WPILib BSD license file in the root directory of this project.

package frc.robot.commands;

import frc.robot.subsystems.DrivetrainSubsystem;
import edu.wpi.first.wpilibj2.command.CommandBase;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

import edu.wpi.first.wpilibj.geometry.Pose2d;
import edu.wpi.first.wpilibj.geometry.Rotation2d;
import edu.wpi.first.wpilibj.geometry.Translation2d;
import edu.wpi.first.wpilibj.trajectory.Trajectory;
import edu.wpi.first.wpilibj.trajectory.TrajectoryConfig;
import edu.wpi.first.wpilibj.trajectory.TrajectoryGenerator;
import edu.wpi.first.wpilibj.kinematics.ChassisSpeeds;
import edu.wpi.first.wpilibj.Timer;
import java.util.List;

public class TrajectoryDriveCommand extends CommandBase {
  //@SuppressWarnings({"PMD.UnusedPrivateField", "PMD.SingularField"})
  private final DrivetrainSubsystem m_subsystem;

  private TrajectoryConfig m_config;
  private Trajectory m_trajectory;

  private final Timer m_timer = new Timer();
  /**
   * Creates a new ExampleCommand.
   *
   * @param subsystem The subsystem used by this command.
   */
  public TrajectoryDriveCommand(
    DrivetrainSubsystem p_subsystem, 
    List<Translation2d> p_interiorWaypoints, 
    Pose2d p_end) 
  {
    m_subsystem = p_subsystem;

    // Use addRequirements() here to declare subsystem dependencies.
    addRequirements(p_subsystem);

    m_config = new TrajectoryConfig(DrivetrainSubsystem.MaxSpeedMetersPerSecond,DrivetrainSubsystem.MaxAccelerationMetersPerSecondSquared)
      // Add kinematics to ensure max speed is actually obeyed
      .setKinematics(m_subsystem.getKinematics());
    
    m_trajectory = TrajectoryGenerator.generateTrajectory(p_subsystem.getPose(), p_interiorWaypoints, p_end, m_config);
  }

  // Called when the command is initially scheduled.
  @Override
  public void initialize() {
    m_timer.reset();
    m_timer.start();
  }

  @Override
  public void execute() {
    double curTime = m_timer.get();
    var desiredState = m_trajectory.sample(curTime);

    double xFF = desiredState.velocityMetersPerSecond * desiredState.poseMeters.getRotation().getCos();
    double yFF = desiredState.velocityMetersPerSecond * desiredState.poseMeters.getRotation().getSin();
    double thetaFF = desiredState.curvatureRadPerMeter;
    var targetChassisSpeeds = ChassisSpeeds.fromFieldRelativeSpeeds(xFF, yFF, thetaFF, m_subsystem.getGyroscopeRotation());
    m_subsystem.drive(targetChassisSpeeds);
  }

  // Called once the command ends or is interrupted.
  @Override
  public void end(boolean interrupted) {
    m_timer.stop();
    m_subsystem.drive(new ChassisSpeeds(0,0,0));
  }

  // Returns true when the command should end.
  @Override
  public boolean isFinished() {
    return m_timer.hasElapsed(m_trajectory.getTotalTimeSeconds());
  }
}
