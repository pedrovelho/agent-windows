/*
 * ################################################################
 *
 * ProActive Parallel Suite(TM): The Java(TM) library for
 *    Parallel, Distributed, Multi-Core Computing for
 *    Enterprise Grids & Clouds
 *
 * Copyright (C) 1997-2011 INRIA/University of
 *                 Nice-Sophia Antipolis/ActiveEon
 * Contact: proactive@ow2.org or contact@activeeon.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Affero General Public License
 * as published by the Free Software Foundation; version 3 of
 * the License.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
 * USA
 *
 * If needed, contact us to obtain a release under GPL Version 2 or 3
 * or a different license than the AGPL.
 *
 *  Initial developer(s):               The ActiveEon Team
 *                        http://www.activeeon.com/
 *  Contributor(s):
 *
 * ################################################################ 
 * $$ACTIVEEON_CONTRIBUTOR$$
 */
package GUI;

import Connections.Connections;
import Connections.Connections.CONNTYPE;
import Planning.Config;
import Planning.Duration;
import Planning.Event;
import Planning.PlanningTime;
import Utils.TimeComputer;
import Model.ModelManager;
import Utils.ExtensionFileFilter;
import Utils.ListNetworkInterfaces;
import java.awt.Dimension;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import javax.imageio.ImageIO;
import javax.swing.DefaultListModel;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.SpinnerModel;
import javax.swing.SpinnerNumberModel;
import javax.swing.filechooser.FileFilter;

/**
 *
 * @author philippe Gouttefarde
 */
public class GUIEditorWindows extends javax.swing.JFrame {

    /*
     * The main class of the graphical Editor.
     * It's getting all listener to user interaction and an instance of ShowApplets()
     */
    
    private void initConnections() {
        
        Connections conn = ModelManager.getCONNECTIONS();
        //LOCAL CONNECTION
        textStarterClass4.setText(conn.getLocal().getJavaStarterClass());
        textNodeName1.setText(conn.getLocal().getNodeName());
        //RM CONNECTION
        textStarterClass2.setText(conn.getRMConn().getJavaStarterClass());
        textNodeName2.setText(conn.getRMConn().getNodeName());
        textRMURL.setText(conn.getRMConn().getUrl());
        textNodeSourceName.setText(conn.getRMConn().getNodeSourceName());
        textCredential.setText(conn.getRMConn().getCredential());
        //CUSTOM CONNECTION
        textStarterClass3.setText(conn.getCustom().getJavaStarterClass());
        
        if(conn.getSelected() == CONNTYPE.LOCAL) {
            radioLocal.setSelected(true);
        }
        else if(conn.getSelected() == CONNTYPE.RM) {
            radioResourceManager.setSelected(true);
        }
        else if(conn.getSelected() == CONNTYPE.CUSTOM) {
            radioCustom.setSelected(true);
        } else {
            radioLocal.setSelected(true);
        }
        
        for (String arg : ModelManager.getCONNECTIONS().getCustom().getArgs()) {
            setArgumentToTheArgumentList(arg);
        }
        
    }
    
    private void initPlanning() {
        
        listPlanning.setModel(new DefaultListModel());
        
        for (Event ev : ModelManager.getEVENTS().getListEvents()) {
            addEventToPlanningTab(ev);
            setEventToPanel(ev);
        }
    }
    
    private void initCustomParameters() {
        String os = System.getProperty("os.name").toLowerCase();
        if(os.indexOf( "win" ) >= 0) {
            labelClassData.setVisible(false);
            spinnerClassData.setVisible(false);
        } else if (os.indexOf( "nix") >=0 || os.indexOf( "nux") >=0) {
            labelMaxCPUUsage.setText("nice:");
            labelProcessPrioriry.setText("ionice:");
            
            comboBoxPriority.setModel(new javax.swing.DefaultComboBoxModel(new String[] { "none", "realtime", "besteffort", "idle" }));
            comboBoxPriority.setSelectedIndex(0);
            comboBoxPriority.addItemListener(new java.awt.event.ItemListener() {
                public void itemStateChanged(java.awt.event.ItemEvent evt) {
                    changePriority(evt);
                }
            });
            
            spinnerMaxCPUUsage.setModel(new javax.swing.SpinnerNumberModel(1, 1, 19, 1));
            spinnerMaxCPUUsage.addChangeListener(new javax.swing.event.ChangeListener() {
                public void stateChanged(javax.swing.event.ChangeEvent evt) {
                    changeMaxCpuUsage(evt);
                }
            });
            spinnerMaxCPUUsage.setToolTipText( "specifies the nice property of the Runtimes process and its chlidren processes during this plan." );
            
            labelForCent.setVisible(false);
        }
        BufferedImage images = null;
        try {
            images = ImageIO.read(this.getClass().getResource( icon ));
        } catch (IOException es) {
            es.printStackTrace();
        }
        this.setIconImage(images);
    }
    
    public void initializeText() {
        
        //Initialized Title
        setTitle(TITLE_MAIN_FRAME);
        
        //Initialize ProActive Home Text
        textProActiveHome.setText( ModelManager.getPROACTIVEHOME() );
        //Initialize JAVAHOME Text
        textJavaHome.setText( ModelManager.getJAVAHOME() );
        //Initialize Script on exit Text
        textScriptLocation.setText( ModelManager.getSCRIPTONEXIT() ); 
        //Initialize Memory Limit ....
        int freeMemory = ((int)Runtime.getRuntime().freeMemory())/1000;
        textMemory.setText( freeMemory + "" );
        //                                      .... and its spinner
        int currentMemory = ModelManager.getMEMORYLIMIT();
        SpinnerModel tmpSpinner = new SpinnerNumberModel(currentMemory,0,freeMemory,1); // current,min,max,step
        splinMemoryLimit.setModel(tmpSpinner);
        
        //Initialize number CPUs
        textCPUs.setText( Runtime.getRuntime().availableProcessors() + "" );
        
        //Initialize protocol
        comboBoxProtocol.setSelectedIndex( ModelManager.convertProtocolToInt(ModelManager.getPROTOCOL()) );
        
        //Initialize JVM Options
        JListJVMOption.setModel(new DefaultListModel());
        for (String param : ModelManager.getJVMOPTIONS()) {
            setOptionToJVMOptionList(param);
        }
        
        //Initialize Planning panel:
        initPlanning();
        
        //Initialize Connections panel:
        initConnections();
        
    }
    
    public GUIEditorWindows(String XMLFile) {
        System.out.println("Starting application...");
        initComponents();
        
        //Init Config
        ModelManager.setXMLFileName(XMLFile);
        if(ModelManager.loadXML(this)) {
            System.out.println("open : " + ModelManager.getXMLFileName() + " [OK] ");
        } else {
            System.out.println("open : " + ModelManager.getXMLFileName() + " [FAIL] ");
        }
        
        //Initialize Process Parameters
        initCustomParameters();
        
        //Initialize data
        initializeText();
        setVisible(true);
    }
    
    public GUIEditorWindows(String XMLFile, String XSDFile) {
        System.out.println("Starting application...");
        initComponents();
        
        //Init Config
        ModelManager.setXMLFileName(XMLFile);
        ModelManager.setXSDCustom(XSDFile);
        if(ModelManager.loadXML(this)) {
            System.out.println("open : " + ModelManager.getXMLFileName() + " [OK] ");
        } else {
            System.out.println("open : " + ModelManager.getXMLFileName() + " [FAIL] ");
        }
        
        //Initialize Process Parameters
        initCustomParameters();
        
        //Initialize data
        initializeText();
        setVisible(true);
    }
    
    /** Creates new form GUIEditorWindows */
    public GUIEditorWindows() {
        System.out.println("Starting application...");
        initComponents();
        
        //Initialize Process Parameters
        initCustomParameters();
        radioLocal.setSelected(true);
    }

    /** This method is called from within the constructor to
     * initialize the form.
     * WARNING: Do NOT modify this code. The content of this method is
     * always regenerated by the Form Editor.
     */
    @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated Code">//GEN-BEGIN:initComponents
    private void initComponents() {

        buttonGroup1 = new javax.swing.ButtonGroup();
        jScrollPane2 = new javax.swing.JScrollPane();
        jList1 = new javax.swing.JList();
        ButtonClose = new javax.swing.JButton();
        ButtonSave = new javax.swing.JButton();
        ButtonSaveAs = new javax.swing.JButton();
        GlobalPanel = new javax.swing.JTabbedPane();
        PanelGeneral = new javax.swing.JPanel();
        PanelConfig = new javax.swing.JPanel();
        buttonBrowseProActiveHome = new javax.swing.JButton();
        buttonBrowseJavaHome = new javax.swing.JButton();
        textProActiveHome = new javax.swing.JTextField();
        textJavaHome = new javax.swing.JTextField();
        checkBoxJavaHome = new javax.swing.JCheckBox();
        buttonAddJVMOpt = new javax.swing.JButton();
        buttonRemoveJVMOpt = new javax.swing.JButton();
        jScrollPane3 = new javax.swing.JScrollPane();
        JListJVMOption = new javax.swing.JList();
        PanelANI = new javax.swing.JPanel();
        buttonUse = new javax.swing.JButton();
        buttonRefresh = new javax.swing.JButton();
        jScrollPane1 = new javax.swing.JScrollPane();
        JListInterfaces = new javax.swing.JList();
        PanelRML = new javax.swing.JPanel();
        labelMemory = new javax.swing.JLabel();
        textMemory = new javax.swing.JLabel();
        labelMemoryLimit = new javax.swing.JLabel();
        splinMemoryLimit = new javax.swing.JSpinner();
        notice1 = new javax.swing.JLabel();
        notice2 = new javax.swing.JLabel();
        PanelMR = new javax.swing.JPanel();
        labelCPUs = new javax.swing.JLabel();
        textCPUs = new javax.swing.JLabel();
        labelNbRuntimes = new javax.swing.JLabel();
        spinNbRuntimes = new javax.swing.JSpinner();
        checkBoxCPUs = new javax.swing.JCheckBox();
        PanelORE = new javax.swing.JPanel();
        buttonBrowseScriptLocation = new javax.swing.JButton();
        textScriptLocation = new javax.swing.JTextField();
        panelRuntimeIncomingProtocol1 = new javax.swing.JPanel();
        labelProtocol1 = new javax.swing.JLabel();
        comboBoxProtocol = new javax.swing.JComboBox();
        labelPortInitialValue1 = new javax.swing.JLabel();
        spinPortInitialValue = new javax.swing.JSpinner();
        PanelConnection = new javax.swing.JPanel();
        panelMultipleConnections = new javax.swing.JTabbedPane();
        panelLocal = new javax.swing.JPanel();
        panelLocalRegistration = new javax.swing.JPanel();
        labelNodeName1 = new javax.swing.JLabel();
        textNodeName1 = new javax.swing.JTextField();
        PanelAdditionnalConf4 = new javax.swing.JPanel();
        labelStarterClass4 = new javax.swing.JLabel();
        textStarterClass4 = new javax.swing.JTextField();
        panelResourceManager = new javax.swing.JPanel();
        panelRMRegistration = new javax.swing.JPanel();
        labelRMURL = new javax.swing.JLabel();
        textRMURL = new javax.swing.JTextField();
        labelNodeName2 = new javax.swing.JLabel();
        textNodeName2 = new javax.swing.JTextField();
        labelNodeSourceName = new javax.swing.JLabel();
        textNodeSourceName = new javax.swing.JTextField();
        panelAuthCredential = new javax.swing.JPanel();
        textCredential = new javax.swing.JTextField();
        buttonBrowseLocation = new javax.swing.JButton();
        PanelAdditionnalConf2 = new javax.swing.JPanel();
        labelStarterClass2 = new javax.swing.JLabel();
        textStarterClass2 = new javax.swing.JTextField();
        panelCustom = new javax.swing.JPanel();
        PanelAdditionnalConf3 = new javax.swing.JPanel();
        labelStarterClass3 = new javax.swing.JLabel();
        textStarterClass3 = new javax.swing.JTextField();
        panelCustom2 = new javax.swing.JPanel();
        labelArguments = new javax.swing.JLabel();
        buttonAdd = new javax.swing.JButton();
        buttonDelete = new javax.swing.JButton();
        buttonSaveArg = new javax.swing.JButton();
        labelArgument = new javax.swing.JLabel();
        textArgument = new javax.swing.JTextField();
        jScrollPane4 = new javax.swing.JScrollPane();
        JlistArguments = new javax.swing.JList();
        radioLocal = new javax.swing.JRadioButton();
        radioResourceManager = new javax.swing.JRadioButton();
        radioCustom = new javax.swing.JRadioButton();
        PanelPlanning = new javax.swing.JPanel();
        panelWeeklyPlanning = new javax.swing.JPanel();
        buttonCreatePlan = new javax.swing.JButton();
        buttonDeletePlan = new javax.swing.JButton();
        buttonShowPlan = new javax.swing.JButton();
        testAeraPlanning = new javax.swing.JScrollPane();
        listPlanning = new javax.swing.JList();
        panelPlanEditor = new javax.swing.JPanel();
        panelStartTime = new javax.swing.JPanel();
        labelStartDay = new javax.swing.JLabel();
        comboBoxStartDay = new javax.swing.JComboBox();
        labelStartHours = new javax.swing.JLabel();
        spinnerStartHours = new javax.swing.JSpinner();
        labelStartMinutes = new javax.swing.JLabel();
        spinnerStartMinutes = new javax.swing.JSpinner();
        labelStartSecondes = new javax.swing.JLabel();
        spinnerStartSecondes = new javax.swing.JSpinner();
        panelDuration = new javax.swing.JPanel();
        labelDurationDays = new javax.swing.JLabel();
        spinnerDurationDays = new javax.swing.JSpinner();
        labelDurationHours = new javax.swing.JLabel();
        spinnerDurationHours = new javax.swing.JSpinner();
        labelDurationMinutes = new javax.swing.JLabel();
        spinnerDurationMinutes = new javax.swing.JSpinner();
        labelDurationSecondes = new javax.swing.JLabel();
        spinnerDurationSecondes = new javax.swing.JSpinner();
        panelProcessManagement = new javax.swing.JPanel();
        labelProcessPrioriry = new javax.swing.JLabel();
        comboBoxPriority = new javax.swing.JComboBox();
        labelMaxCPUUsage = new javax.swing.JLabel();
        spinnerMaxCPUUsage = new javax.swing.JSpinner();
        labelForCent = new javax.swing.JLabel();
        labelClassData = new javax.swing.JLabel();
        spinnerClassData = new javax.swing.JSpinner();
        checkBoxAlwaysAvailable = new javax.swing.JCheckBox();

        jList1.setModel(new javax.swing.AbstractListModel() {
            String[] strings = { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" };
            public int getSize() { return strings.length; }
            public Object getElementAt(int i) { return strings[i]; }
        });
        jScrollPane2.setViewportView(jList1);

        setDefaultCloseOperation(javax.swing.WindowConstants.DISPOSE_ON_CLOSE);
        setTitle("GUI Editor");
        setFont(new java.awt.Font("DejaVu Serif", 0, 12)); // NOI18N
        setLocationByPlatform(true);
        setResizable(false);

        ButtonClose.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        ButtonClose.setText("Close");
        ButtonClose.setToolTipText("closes this windows.");
        ButtonClose.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                close(evt);
            }
        });

        ButtonSave.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        ButtonSave.setText("Save");
        ButtonSave.setToolTipText("saves modifications.");
        ButtonSave.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                saveProperties(evt);
            }
        });

        ButtonSaveAs.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        ButtonSaveAs.setText("Save As");
        ButtonSaveAs.setToolTipText("saves thecurrent configuration under the user specified filename. ");
        ButtonSaveAs.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                saveAsProperties(evt);
            }
        });

        GlobalPanel.setFont(new java.awt.Font("TakaoPGothic", 0, 11));

        PanelConfig.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "ProActive Configuration", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        PanelConfig.setFont(new java.awt.Font("TakaoPGothic", 0, 11));

        buttonBrowseProActiveHome.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonBrowseProActiveHome.setText("Browse ProActive Home");
        buttonBrowseProActiveHome.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                browseProActiveHome(evt);
            }
        });

        buttonBrowseJavaHome.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonBrowseJavaHome.setText("Browse Java Home");
        buttonBrowseJavaHome.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                browseJavaHome(evt);
            }
        });

        textProActiveHome.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textProActiveHome.setText("set the proactive home");
        textProActiveHome.setToolTipText("location of the ProActive or Scheduler home.");

        textJavaHome.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textJavaHome.setText("set the java home");
        textJavaHome.setToolTipText("Java home location.");

        checkBoxJavaHome.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        checkBoxJavaHome.setText("Use system-wide Java Home");
        checkBoxJavaHome.setToolTipText("use JVM location specified by the JAVA_HOME environnement variable.");
        checkBoxJavaHome.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                JavaHomeSystemChooser(evt);
            }
        });

        buttonAddJVMOpt.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonAddJVMOpt.setText("Add JVM Option");
        buttonAddJVMOpt.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                addJVMOption(evt);
            }
        });

        buttonRemoveJVMOpt.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonRemoveJVMOpt.setText("Remove JVM Option");
        buttonRemoveJVMOpt.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                removeJVMOption(evt);
            }
        });

        JListJVMOption.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        JListJVMOption.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        JListJVMOption.setToolTipText("if the parameter contain ${rank} it will be automatically replaced by the Runtime rank. ");
        JListJVMOption.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                selectJVMOption(evt);
            }
        });
        jScrollPane3.setViewportView(JListJVMOption);

        org.jdesktop.layout.GroupLayout PanelConfigLayout = new org.jdesktop.layout.GroupLayout(PanelConfig);
        PanelConfig.setLayout(PanelConfigLayout);
        PanelConfigLayout.setHorizontalGroup(
            PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelConfigLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, buttonAddJVMOpt, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 129, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, buttonBrowseJavaHome)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, buttonBrowseProActiveHome)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, buttonRemoveJVMOpt))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(jScrollPane3, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 610, Short.MAX_VALUE)
                    .add(checkBoxJavaHome)
                    .add(textProActiveHome, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 610, Short.MAX_VALUE)
                    .add(textJavaHome, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 610, Short.MAX_VALUE))
                .addContainerGap())
        );

        PanelConfigLayout.linkSize(new java.awt.Component[] {buttonAddJVMOpt, buttonBrowseJavaHome, buttonBrowseProActiveHome, buttonRemoveJVMOpt}, org.jdesktop.layout.GroupLayout.HORIZONTAL);

        PanelConfigLayout.setVerticalGroup(
            PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelConfigLayout.createSequentialGroup()
                .add(PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(textProActiveHome, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(buttonBrowseProActiveHome))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(textJavaHome, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(buttonBrowseJavaHome))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(checkBoxJavaHome)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(PanelConfigLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelConfigLayout.createSequentialGroup()
                        .add(buttonAddJVMOpt)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(buttonRemoveJVMOpt))
                    .add(jScrollPane3, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 64, Short.MAX_VALUE))
                .addContainerGap())
        );

        PanelANI.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Available Network Interfaces", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        PanelANI.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N

        buttonUse.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonUse.setText("Use");
        buttonUse.setEnabled(false);
        buttonUse.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                useInterface(evt);
            }
        });

        buttonRefresh.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonRefresh.setText("Refresh");
        buttonRefresh.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                refreshInterfacesList(evt);
            }
        });

        JListInterfaces.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        JListInterfaces.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        JListInterfaces.setToolTipText("the list of available network interface.");
        JListInterfaces.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                selectNetworkInterface(evt);
            }
        });
        jScrollPane1.setViewportView(JListInterfaces);

        org.jdesktop.layout.GroupLayout PanelANILayout = new org.jdesktop.layout.GroupLayout(PanelANI);
        PanelANI.setLayout(PanelANILayout);
        PanelANILayout.setHorizontalGroup(
            PanelANILayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, PanelANILayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelANILayout.createParallelGroup(org.jdesktop.layout.GroupLayout.TRAILING)
                    .add(org.jdesktop.layout.GroupLayout.LEADING, jScrollPane1, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 307, Short.MAX_VALUE)
                    .add(PanelANILayout.createSequentialGroup()
                        .add(buttonUse)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(buttonRefresh)))
                .addContainerGap())
        );

        PanelANILayout.linkSize(new java.awt.Component[] {buttonRefresh, buttonUse}, org.jdesktop.layout.GroupLayout.HORIZONTAL);

        PanelANILayout.setVerticalGroup(
            PanelANILayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, PanelANILayout.createSequentialGroup()
                .add(jScrollPane1, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 217, Short.MAX_VALUE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.UNRELATED)
                .add(PanelANILayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(buttonUse)
                    .add(buttonRefresh)))
        );

        PanelRML.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Runtime Memory Limit (Mbytes)", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        PanelRML.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N

        labelMemory.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelMemory.setText("Available Physical Memory :");

        textMemory.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        textMemory.setText("951");

        labelMemoryLimit.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelMemoryLimit.setText("Memory Limit :");

        splinMemoryLimit.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        splinMemoryLimit.setModel(new javax.swing.SpinnerNumberModel(Integer.valueOf(0), Integer.valueOf(0), null, Integer.valueOf(1)));
        splinMemoryLimit.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                listenSpinMemory(evt);
            }
        });

        notice1.setFont(new java.awt.Font("Tahoma", 2, 11));
        notice1.setForeground(new java.awt.Color(102, 102, 102));
        notice1.setText("Notice : 0 means no memory limit and");

        notice2.setFont(new java.awt.Font("Tahoma", 2, 11));
        notice2.setForeground(new java.awt.Color(102, 102, 102));
        notice2.setText("128 is required for a ProActive Runtime");

        org.jdesktop.layout.GroupLayout PanelRMLLayout = new org.jdesktop.layout.GroupLayout(PanelRML);
        PanelRML.setLayout(PanelRMLLayout);
        PanelRMLLayout.setHorizontalGroup(
            PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelRMLLayout.createSequentialGroup()
                .add(PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelRMLLayout.createSequentialGroup()
                        .addContainerGap()
                        .add(PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(org.jdesktop.layout.GroupLayout.TRAILING, labelMemoryLimit)
                            .add(org.jdesktop.layout.GroupLayout.TRAILING, labelMemory))
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(splinMemoryLimit, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 67, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                            .add(textMemory)))
                    .add(PanelRMLLayout.createSequentialGroup()
                        .add(19, 19, 19)
                        .add(PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.TRAILING, false)
                            .add(org.jdesktop.layout.GroupLayout.LEADING, notice1, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                            .add(org.jdesktop.layout.GroupLayout.LEADING, notice2))))
                .addContainerGap(19, Short.MAX_VALUE))
        );
        PanelRMLLayout.setVerticalGroup(
            PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelRMLLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelMemory)
                    .add(textMemory))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(PanelRMLLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelMemoryLimit)
                    .add(splinMemoryLimit, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.UNRELATED)
                .add(notice1)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(notice2)
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        PanelMR.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Multi-Runtime", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        PanelMR.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N

        labelCPUs.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelCPUs.setText("Available CPUs :");

        textCPUs.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        textCPUs.setText("4");

        labelNbRuntimes.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelNbRuntimes.setText("Nb Runtimes :");

        spinNbRuntimes.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        spinNbRuntimes.setModel(new javax.swing.SpinnerNumberModel(0, 0, 32, 1));

        checkBoxCPUs.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        checkBoxCPUs.setText("Use all available CPUs");
        checkBoxCPUs.setToolTipText("Will spawn as many Runtimes as available CPUs. The number of CPUs is detected automatically.");
        checkBoxCPUs.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                CPUsChooser(evt);
            }
        });

        org.jdesktop.layout.GroupLayout PanelMRLayout = new org.jdesktop.layout.GroupLayout(PanelMR);
        PanelMR.setLayout(PanelMRLayout);
        PanelMRLayout.setHorizontalGroup(
            PanelMRLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelMRLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelMRLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelMRLayout.createSequentialGroup()
                        .add(PanelMRLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(org.jdesktop.layout.GroupLayout.TRAILING, labelNbRuntimes)
                            .add(org.jdesktop.layout.GroupLayout.TRAILING, labelCPUs))
                        .add(18, 18, 18)
                        .add(PanelMRLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(spinNbRuntimes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 63, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                            .add(textCPUs)))
                    .add(checkBoxCPUs))
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );
        PanelMRLayout.setVerticalGroup(
            PanelMRLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelMRLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelMRLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelMRLayout.createSequentialGroup()
                        .add(labelCPUs)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(labelNbRuntimes))
                    .add(PanelMRLayout.createSequentialGroup()
                        .add(textCPUs)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(spinNbRuntimes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 26, Short.MAX_VALUE)
                .add(checkBoxCPUs)
                .addContainerGap())
        );

        PanelORE.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "On Runtime Exit", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        PanelORE.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N

        buttonBrowseScriptLocation.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonBrowseScriptLocation.setText("Browse Script Location");
        buttonBrowseScriptLocation.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                browseScriptOnExit(evt);
            }
        });

        textScriptLocation.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textScriptLocation.setToolTipText("location of the script executed  after a Runtime termitates.");

        org.jdesktop.layout.GroupLayout PanelORELayout = new org.jdesktop.layout.GroupLayout(PanelORE);
        PanelORE.setLayout(PanelORELayout);
        PanelORELayout.setHorizontalGroup(
            PanelORELayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelORELayout.createSequentialGroup()
                .addContainerGap()
                .add(buttonBrowseScriptLocation)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.UNRELATED)
                .add(textScriptLocation, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 261, Short.MAX_VALUE)
                .addContainerGap())
        );
        PanelORELayout.setVerticalGroup(
            PanelORELayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelORELayout.createSequentialGroup()
                .add(PanelORELayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(buttonBrowseScriptLocation)
                    .add(textScriptLocation, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        panelRuntimeIncomingProtocol1.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Runtime Incoming Protocol", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        panelRuntimeIncomingProtocol1.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N

        labelProtocol1.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelProtocol1.setText("Protocol:");

        comboBoxProtocol.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        comboBoxProtocol.setModel(new javax.swing.DefaultComboBoxModel(new String[] { "undefined", "rmi", "http", "pamr", "pnp", "pnps" }));
        comboBoxProtocol.setToolTipText("defines the protocol to used by the Runtime.");

        labelPortInitialValue1.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelPortInitialValue1.setText("Port Initial Value:");

        spinPortInitialValue.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinPortInitialValue.setToolTipText("defines the value of the \"Dproactive.SELECTED_PROTOCOL.port\" protperty. This value will always be increased by 1 or more if there is more than one Runtime.  ");

        org.jdesktop.layout.GroupLayout panelRuntimeIncomingProtocol1Layout = new org.jdesktop.layout.GroupLayout(panelRuntimeIncomingProtocol1);
        panelRuntimeIncomingProtocol1.setLayout(panelRuntimeIncomingProtocol1Layout);
        panelRuntimeIncomingProtocol1Layout.setHorizontalGroup(
            panelRuntimeIncomingProtocol1Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelRuntimeIncomingProtocol1Layout.createSequentialGroup()
                .addContainerGap()
                .add(labelProtocol1)
                .add(18, 18, 18)
                .add(comboBoxProtocol, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 137, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 77, Short.MAX_VALUE)
                .add(labelPortInitialValue1)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinPortInitialValue, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 61, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addContainerGap())
        );
        panelRuntimeIncomingProtocol1Layout.setVerticalGroup(
            panelRuntimeIncomingProtocol1Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelRuntimeIncomingProtocol1Layout.createSequentialGroup()
                .add(panelRuntimeIncomingProtocol1Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(comboBoxProtocol, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelProtocol1)
                    .add(labelPortInitialValue1)
                    .add(spinPortInitialValue, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        org.jdesktop.layout.GroupLayout PanelGeneralLayout = new org.jdesktop.layout.GroupLayout(PanelGeneral);
        PanelGeneral.setLayout(PanelGeneralLayout);
        PanelGeneralLayout.setHorizontalGroup(
            PanelGeneralLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelGeneralLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelGeneralLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelConfig, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(PanelGeneralLayout.createSequentialGroup()
                        .add(PanelGeneralLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(PanelORE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                            .add(panelRuntimeIncomingProtocol1, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                            .add(PanelGeneralLayout.createSequentialGroup()
                                .add(PanelRML, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                                .add(PanelMR, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)))
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(PanelANI, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)))
                .addContainerGap())
        );
        PanelGeneralLayout.setVerticalGroup(
            PanelGeneralLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelGeneralLayout.createSequentialGroup()
                .add(PanelConfig, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(PanelGeneralLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelGeneralLayout.createSequentialGroup()
                        .add(PanelORE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(PanelGeneralLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(PanelMR, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                            .add(PanelRML, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(panelRuntimeIncomingProtocol1, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                    .add(PanelANI, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .add(50, 50, 50))
        );

        GlobalPanel.addTab("General", PanelGeneral);

        panelMultipleConnections.setTabPlacement(javax.swing.JTabbedPane.LEFT);
        panelMultipleConnections.setFont(new java.awt.Font("TakaoPGothic", 0, 11));

        panelLocalRegistration.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Local Registration", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        panelLocalRegistration.setFont(new java.awt.Font("TakaoPGothic", 0, 11));

        labelNodeName1.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelNodeName1.setText("Node Name:");

        textNodeName1.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textNodeName1.setToolTipText("defines the name of the node. The node will be register under url like URL://IP_OR_HOSTNAME:PORT/NODE_NAME ");

        org.jdesktop.layout.GroupLayout panelLocalRegistrationLayout = new org.jdesktop.layout.GroupLayout(panelLocalRegistration);
        panelLocalRegistration.setLayout(panelLocalRegistrationLayout);
        panelLocalRegistrationLayout.setHorizontalGroup(
            panelLocalRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelLocalRegistrationLayout.createSequentialGroup()
                .addContainerGap()
                .add(labelNodeName1)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(textNodeName1, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 493, Short.MAX_VALUE)
                .addContainerGap())
        );
        panelLocalRegistrationLayout.setVerticalGroup(
            panelLocalRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelLocalRegistrationLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelLocalRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelNodeName1)
                    .add(textNodeName1, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        PanelAdditionnalConf4.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Local Registration", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N
        PanelAdditionnalConf4.setFont(new java.awt.Font("TakaoPGothic", 0, 11));

        labelStarterClass4.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStarterClass4.setText("Java Starter Class:");

        textStarterClass4.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textStarterClass4.setText("org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter");
        textStarterClass4.setToolTipText("defines the java class to run.");

        org.jdesktop.layout.GroupLayout PanelAdditionnalConf4Layout = new org.jdesktop.layout.GroupLayout(PanelAdditionnalConf4);
        PanelAdditionnalConf4.setLayout(PanelAdditionnalConf4Layout);
        PanelAdditionnalConf4Layout.setHorizontalGroup(
            PanelAdditionnalConf4Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelAdditionnalConf4Layout.createSequentialGroup()
                .addContainerGap()
                .add(labelStarterClass4)
                .add(18, 18, 18)
                .add(textStarterClass4, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 445, Short.MAX_VALUE)
                .addContainerGap())
        );
        PanelAdditionnalConf4Layout.setVerticalGroup(
            PanelAdditionnalConf4Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelAdditionnalConf4Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                .add(labelStarterClass4)
                .add(textStarterClass4, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
        );

        org.jdesktop.layout.GroupLayout panelLocalLayout = new org.jdesktop.layout.GroupLayout(panelLocal);
        panelLocal.setLayout(panelLocalLayout);
        panelLocalLayout.setHorizontalGroup(
            panelLocalLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelLocalLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelLocalLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.TRAILING)
                    .add(org.jdesktop.layout.GroupLayout.LEADING, panelLocalRegistration, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.LEADING, PanelAdditionnalConf4, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap())
        );
        panelLocalLayout.setVerticalGroup(
            panelLocalLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelLocalLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelLocalRegistration, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 300, Short.MAX_VALUE)
                .add(PanelAdditionnalConf4, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addContainerGap())
        );

        panelMultipleConnections.addTab("Local Registration", panelLocal);

        panelRMRegistration.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Resource Manager Registration", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelRMURL.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelRMURL.setText("Resource Manager URL:");

        textRMURL.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textRMURL.setText("rmi://localhost:1099");
        textRMURL.setToolTipText("example : URL://IP_OR_HOSTNAME:PORT/");

        labelNodeName2.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelNodeName2.setText("Node Name:");

        textNodeName2.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textNodeName2.setToolTipText("the name of the node without whitespaces.");

        labelNodeSourceName.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelNodeSourceName.setText("Node Source Name:");

        textNodeSourceName.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textNodeSourceName.setToolTipText("the name of the node source without whitespaces.");

        org.jdesktop.layout.GroupLayout panelRMRegistrationLayout = new org.jdesktop.layout.GroupLayout(panelRMRegistration);
        panelRMRegistration.setLayout(panelRMRegistrationLayout);
        panelRMRegistrationLayout.setHorizontalGroup(
            panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelRMRegistrationLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, labelRMURL)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, labelNodeName2)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, labelNodeSourceName))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, textNodeName2, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 349, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, textNodeSourceName)
                    .add(textRMURL, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 505, Short.MAX_VALUE))
                .addContainerGap())
        );
        panelRMRegistrationLayout.setVerticalGroup(
            panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelRMRegistrationLayout.createSequentialGroup()
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                .add(panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelRMURL)
                    .add(textRMURL, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelNodeName2)
                    .add(textNodeName2, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .add(8, 8, 8)
                .add(panelRMRegistrationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelNodeSourceName)
                    .add(textNodeSourceName, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)))
        );

        panelRMRegistrationLayout.linkSize(new java.awt.Component[] {textNodeName2, textNodeSourceName, textRMURL}, org.jdesktop.layout.GroupLayout.VERTICAL);

        panelAuthCredential.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Resource Manager Registration", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        textCredential.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textCredential.setToolTipText("the location of the file contains the credentials.");

        buttonBrowseLocation.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonBrowseLocation.setText("Browse Location");
        buttonBrowseLocation.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                browseCredential(evt);
            }
        });

        org.jdesktop.layout.GroupLayout panelAuthCredentialLayout = new org.jdesktop.layout.GroupLayout(panelAuthCredential);
        panelAuthCredential.setLayout(panelAuthCredentialLayout);
        panelAuthCredentialLayout.setHorizontalGroup(
            panelAuthCredentialLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelAuthCredentialLayout.createSequentialGroup()
                .add(buttonBrowseLocation)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(textCredential, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 444, Short.MAX_VALUE)
                .addContainerGap())
        );
        panelAuthCredentialLayout.setVerticalGroup(
            panelAuthCredentialLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelAuthCredentialLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelAuthCredentialLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(textCredential, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(buttonBrowseLocation))
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        PanelAdditionnalConf2.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Resource Manager Registration", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelStarterClass2.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStarterClass2.setText("Java Starter Class:");

        textStarterClass2.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textStarterClass2.setText("org.ow2.proactive.resourcemanager.utils.RMNodeStarter");
        textStarterClass2.setToolTipText("defines the java class ton run.");

        org.jdesktop.layout.GroupLayout PanelAdditionnalConf2Layout = new org.jdesktop.layout.GroupLayout(PanelAdditionnalConf2);
        PanelAdditionnalConf2.setLayout(PanelAdditionnalConf2Layout);
        PanelAdditionnalConf2Layout.setHorizontalGroup(
            PanelAdditionnalConf2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelAdditionnalConf2Layout.createSequentialGroup()
                .addContainerGap()
                .add(labelStarterClass2)
                .add(18, 18, 18)
                .add(textStarterClass2, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 445, Short.MAX_VALUE)
                .addContainerGap())
        );
        PanelAdditionnalConf2Layout.setVerticalGroup(
            PanelAdditionnalConf2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelAdditionnalConf2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                .add(labelStarterClass2)
                .add(textStarterClass2, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
        );

        org.jdesktop.layout.GroupLayout panelResourceManagerLayout = new org.jdesktop.layout.GroupLayout(panelResourceManager);
        panelResourceManager.setLayout(panelResourceManagerLayout);
        panelResourceManagerLayout.setHorizontalGroup(
            panelResourceManagerLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelResourceManagerLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelResourceManagerLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(PanelAdditionnalConf2, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(panelRMRegistration, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 591, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelAuthCredential, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap())
        );
        panelResourceManagerLayout.setVerticalGroup(
            panelResourceManagerLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelResourceManagerLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelRMRegistration, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelAuthCredential, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 173, Short.MAX_VALUE)
                .add(PanelAdditionnalConf2, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addContainerGap())
        );

        panelMultipleConnections.addTab("Resource Manager Registration", panelResourceManager);

        PanelAdditionnalConf3.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Custom", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelStarterClass3.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStarterClass3.setText("Java Starter Class:");

        textStarterClass3.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textStarterClass3.setText("user.Starter");
        textStarterClass3.setToolTipText("defines the java class to run.");

        org.jdesktop.layout.GroupLayout PanelAdditionnalConf3Layout = new org.jdesktop.layout.GroupLayout(PanelAdditionnalConf3);
        PanelAdditionnalConf3.setLayout(PanelAdditionnalConf3Layout);
        PanelAdditionnalConf3Layout.setHorizontalGroup(
            PanelAdditionnalConf3Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelAdditionnalConf3Layout.createSequentialGroup()
                .addContainerGap()
                .add(labelStarterClass3)
                .add(18, 18, 18)
                .add(textStarterClass3, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 445, Short.MAX_VALUE)
                .addContainerGap())
        );
        PanelAdditionnalConf3Layout.setVerticalGroup(
            PanelAdditionnalConf3Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelAdditionnalConf3Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                .add(labelStarterClass3)
                .add(textStarterClass3, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
        );

        panelCustom2.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Custom", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelArguments.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelArguments.setText("Arguments:");

        buttonAdd.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonAdd.setText("Add");
        buttonAdd.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                addArgumentToCustomConnexion(evt);
            }
        });

        buttonDelete.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonDelete.setText("Delete");
        buttonDelete.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                removeArgument(evt);
            }
        });

        buttonSaveArg.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        buttonSaveArg.setText("Save Arg");

        labelArgument.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelArgument.setText("Argument:");

        textArgument.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        textArgument.setToolTipText("enter an arguments without whitespaces.");

        JlistArguments.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        JlistArguments.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        JlistArguments.setToolTipText("a list od arguments will be passed as parameters to the Java Starter Class. ");
        JlistArguments.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                selectArgument(evt);
            }
        });
        jScrollPane4.setViewportView(JlistArguments);

        org.jdesktop.layout.GroupLayout panelCustom2Layout = new org.jdesktop.layout.GroupLayout(panelCustom2);
        panelCustom2.setLayout(panelCustom2Layout);
        panelCustom2Layout.setHorizontalGroup(
            panelCustom2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelCustom2Layout.createSequentialGroup()
                .addContainerGap()
                .add(panelCustom2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(labelArguments)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelCustom2Layout.createSequentialGroup()
                        .add(jScrollPane4, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 470, Short.MAX_VALUE)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(panelCustom2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                            .add(buttonSaveArg)
                            .add(buttonDelete)
                            .add(buttonAdd)))
                    .add(panelCustom2Layout.createSequentialGroup()
                        .add(labelArgument)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(textArgument, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 501, Short.MAX_VALUE)))
                .addContainerGap())
        );

        panelCustom2Layout.linkSize(new java.awt.Component[] {buttonAdd, buttonDelete, buttonSaveArg}, org.jdesktop.layout.GroupLayout.HORIZONTAL);

        panelCustom2Layout.setVerticalGroup(
            panelCustom2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelCustom2Layout.createSequentialGroup()
                .add(labelArguments)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelCustom2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING, false)
                    .add(panelCustom2Layout.createSequentialGroup()
                        .add(buttonAdd)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(buttonDelete)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(buttonSaveArg))
                    .add(jScrollPane4, 0, 0, Short.MAX_VALUE))
                .add(21, 21, 21)
                .add(panelCustom2Layout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelArgument)
                    .add(textArgument, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addContainerGap(19, Short.MAX_VALUE))
        );

        org.jdesktop.layout.GroupLayout panelCustomLayout = new org.jdesktop.layout.GroupLayout(panelCustom);
        panelCustom.setLayout(panelCustomLayout);
        panelCustomLayout.setHorizontalGroup(
            panelCustomLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelCustomLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelCustomLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelCustom2, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, PanelAdditionnalConf3, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap())
        );
        panelCustomLayout.setVerticalGroup(
            panelCustomLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelCustomLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelCustom2, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 180, Short.MAX_VALUE)
                .add(PanelAdditionnalConf3, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addContainerGap())
        );

        panelMultipleConnections.addTab("Custom", panelCustom);

        buttonGroup1.add(radioLocal);
        radioLocal.setToolTipText("enable Local Registration.");
        radioLocal.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                selectLocalPanel(evt);
            }
        });

        buttonGroup1.add(radioResourceManager);
        radioResourceManager.setToolTipText("enable Resource Manager Registration.");
        radioResourceManager.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                selectRMPanel(evt);
            }
        });

        buttonGroup1.add(radioCustom);
        radioCustom.setToolTipText("enable a user defined connexion.");
        radioCustom.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                selectCustomPanel(evt);
            }
        });

        org.jdesktop.layout.GroupLayout PanelConnectionLayout = new org.jdesktop.layout.GroupLayout(PanelConnection);
        PanelConnection.setLayout(PanelConnectionLayout);
        PanelConnectionLayout.setHorizontalGroup(
            PanelConnectionLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelConnectionLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelConnectionLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.TRAILING)
                    .add(radioLocal)
                    .add(radioResourceManager)
                    .add(radioCustom))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelMultipleConnections, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 784, Short.MAX_VALUE)
                .addContainerGap())
        );
        PanelConnectionLayout.setVerticalGroup(
            PanelConnectionLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(PanelConnectionLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelConnectionLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(panelMultipleConnections, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 453, Short.MAX_VALUE)
                    .add(PanelConnectionLayout.createSequentialGroup()
                        .add(radioLocal)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(radioResourceManager)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(radioCustom)))
                .addContainerGap())
        );

        GlobalPanel.addTab("Connection", PanelConnection);

        panelWeeklyPlanning.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Weekly Planning", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        buttonCreatePlan.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        buttonCreatePlan.setText("Create plan");
        buttonCreatePlan.setToolTipText("creates a new plan.");
        buttonCreatePlan.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                createPlan(evt);
            }
        });

        buttonDeletePlan.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        buttonDeletePlan.setText("Delete");
        buttonDeletePlan.setToolTipText("deletes the selected plan.");
        buttonDeletePlan.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                deletePlan(evt);
            }
        });

        buttonShowPlan.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        buttonShowPlan.setText("Show");
        buttonShowPlan.setToolTipText("shows the charts that represents the weekly planning.");
        buttonShowPlan.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                showGraphic(evt);
            }
        });

        listPlanning.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        listPlanning.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        listPlanning.setToolTipText("the list of weekly plans.");
        listPlanning.addListSelectionListener(new javax.swing.event.ListSelectionListener() {
            public void valueChanged(javax.swing.event.ListSelectionEvent evt) {
                SelectElement(evt);
            }
        });
        testAeraPlanning.setViewportView(listPlanning);

        org.jdesktop.layout.GroupLayout panelWeeklyPlanningLayout = new org.jdesktop.layout.GroupLayout(panelWeeklyPlanning);
        panelWeeklyPlanning.setLayout(panelWeeklyPlanningLayout);
        panelWeeklyPlanningLayout.setHorizontalGroup(
            panelWeeklyPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelWeeklyPlanningLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelWeeklyPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(testAeraPlanning, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 264, Short.MAX_VALUE)
                    .add(panelWeeklyPlanningLayout.createSequentialGroup()
                        .add(buttonCreatePlan, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 95, Short.MAX_VALUE)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(buttonDeletePlan, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 81, Short.MAX_VALUE)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(buttonShowPlan, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 76, Short.MAX_VALUE)))
                .addContainerGap())
        );
        panelWeeklyPlanningLayout.setVerticalGroup(
            panelWeeklyPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelWeeklyPlanningLayout.createSequentialGroup()
                .addContainerGap()
                .add(testAeraPlanning, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 359, Short.MAX_VALUE)
                .add(18, 18, 18)
                .add(panelWeeklyPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(buttonCreatePlan)
                    .add(buttonShowPlan)
                    .add(buttonDeletePlan))
                .addContainerGap())
        );

        panelPlanEditor.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Weekly Planning", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        panelStartTime.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Weekly Planning", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelStartDay.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStartDay.setText("Day:");

        comboBoxStartDay.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        comboBoxStartDay.setModel(new javax.swing.DefaultComboBoxModel(new String[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }));
        comboBoxStartDay.setToolTipText("specific the start day of the week.");
        comboBoxStartDay.addItemListener(new java.awt.event.ItemListener() {
            public void itemStateChanged(java.awt.event.ItemEvent evt) {
                changeDay(evt);
            }
        });

        labelStartHours.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStartHours.setText("Hours:");

        spinnerStartHours.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerStartHours.setModel(new javax.swing.SpinnerNumberModel(0, 0, 23, 1));
        spinnerStartHours.setToolTipText("specific the start hour.");
        spinnerStartHours.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeHour(evt);
            }
        });

        labelStartMinutes.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStartMinutes.setText("Minutes");

        spinnerStartMinutes.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerStartMinutes.setModel(new javax.swing.SpinnerNumberModel(0, 0, 59, 1));
        spinnerStartMinutes.setToolTipText("specific the start minute.");
        spinnerStartMinutes.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeMinutes(evt);
            }
        });

        labelStartSecondes.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelStartSecondes.setText("Seconds");

        spinnerStartSecondes.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerStartSecondes.setModel(new javax.swing.SpinnerNumberModel(0, 0, 59, 1));
        spinnerStartSecondes.setToolTipText("specific the start second.");
        spinnerStartSecondes.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeSeconds(evt);
            }
        });

        org.jdesktop.layout.GroupLayout panelStartTimeLayout = new org.jdesktop.layout.GroupLayout(panelStartTime);
        panelStartTime.setLayout(panelStartTimeLayout);
        panelStartTimeLayout.setHorizontalGroup(
            panelStartTimeLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelStartTimeLayout.createSequentialGroup()
                .addContainerGap()
                .add(labelStartDay)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(comboBoxStartDay, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 90, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 46, Short.MAX_VALUE)
                .add(labelStartHours)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerStartHours, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 37, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .add(18, 18, 18)
                .add(labelStartMinutes)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerStartMinutes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 37, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .add(18, 18, 18)
                .add(labelStartSecondes)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerStartSecondes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
        );
        panelStartTimeLayout.setVerticalGroup(
            panelStartTimeLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelStartTimeLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelStartTimeLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(spinnerStartSecondes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelStartSecondes)
                    .add(spinnerStartHours, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelStartHours)
                    .add(spinnerStartMinutes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelStartMinutes)
                    .add(comboBoxStartDay, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelStartDay))
                .addContainerGap(17, Short.MAX_VALUE))
        );

        panelDuration.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Weekly Planning", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelDurationDays.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelDurationDays.setText("Days:");

        spinnerDurationDays.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerDurationDays.setModel(new javax.swing.SpinnerNumberModel(0, 0, 6, 1));
        spinnerDurationDays.setToolTipText("the number of duration days.");
        spinnerDurationDays.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeDurationDay(evt);
            }
        });

        labelDurationHours.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelDurationHours.setText("Hours:");

        spinnerDurationHours.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerDurationHours.setModel(new javax.swing.SpinnerNumberModel(0, 0, 23, 1));
        spinnerDurationHours.setToolTipText("the number of duration hours.");
        spinnerDurationHours.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeDurationHour(evt);
            }
        });

        labelDurationMinutes.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelDurationMinutes.setText("Minutes");

        spinnerDurationMinutes.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerDurationMinutes.setModel(new javax.swing.SpinnerNumberModel(0, 0, 59, 1));
        spinnerDurationMinutes.setToolTipText("the number of duration minutes.");
        spinnerDurationMinutes.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeDurationMinutes(evt);
            }
        });

        labelDurationSecondes.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelDurationSecondes.setText("Seconds");

        spinnerDurationSecondes.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerDurationSecondes.setModel(new javax.swing.SpinnerNumberModel(0, 0, 59, 1));
        spinnerDurationSecondes.setToolTipText("the number of duration seconds.");
        spinnerDurationSecondes.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeDurationSecond(evt);
            }
        });

        org.jdesktop.layout.GroupLayout panelDurationLayout = new org.jdesktop.layout.GroupLayout(panelDuration);
        panelDuration.setLayout(panelDurationLayout);
        panelDurationLayout.setHorizontalGroup(
            panelDurationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelDurationLayout.createSequentialGroup()
                .addContainerGap()
                .add(labelDurationDays)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerDurationDays, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 77, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 53, Short.MAX_VALUE)
                .add(labelDurationHours)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerDurationHours, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 37, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .add(18, 18, 18)
                .add(labelDurationMinutes)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerDurationMinutes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 37, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .add(18, 18, 18)
                .add(labelDurationSecondes)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(spinnerDurationSecondes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
        );
        panelDurationLayout.setVerticalGroup(
            panelDurationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelDurationLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelDurationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(spinnerDurationDays, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelDurationDays)
                    .add(panelDurationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                        .add(spinnerDurationHours, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                        .add(labelDurationHours)
                        .add(spinnerDurationMinutes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                        .add(labelDurationMinutes))
                    .add(panelDurationLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                        .add(spinnerDurationSecondes, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                        .add(labelDurationSecondes)))
                .addContainerGap(17, Short.MAX_VALUE))
        );

        org.jdesktop.layout.GroupLayout panelPlanEditorLayout = new org.jdesktop.layout.GroupLayout(panelPlanEditor);
        panelPlanEditor.setLayout(panelPlanEditorLayout);
        panelPlanEditorLayout.setHorizontalGroup(
            panelPlanEditorLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, panelPlanEditorLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelPlanEditorLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelStartTime, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelDuration, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap())
        );

        panelPlanEditorLayout.linkSize(new java.awt.Component[] {panelDuration, panelStartTime}, org.jdesktop.layout.GroupLayout.HORIZONTAL);

        panelPlanEditorLayout.setVerticalGroup(
            panelPlanEditorLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelPlanEditorLayout.createSequentialGroup()
                .add(panelStartTime, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelDuration, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        panelPlanEditorLayout.linkSize(new java.awt.Component[] {panelDuration, panelStartTime}, org.jdesktop.layout.GroupLayout.VERTICAL);

        panelProcessManagement.setBorder(javax.swing.BorderFactory.createTitledBorder(null, "Weekly Planning", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("TakaoPGothic", 0, 11))); // NOI18N

        labelProcessPrioriry.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelProcessPrioriry.setText("Process Priority:");

        comboBoxPriority.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        comboBoxPriority.setModel(new javax.swing.DefaultComboBoxModel(new String[] { "Idle", "BelowNormal", "Normal", "AboveNormal", "High", "Realtime" }));
        comboBoxPriority.setSelectedIndex(2);
        comboBoxPriority.setToolTipText("specifies the priority of the Runtimes process and its chlidren processes during this plan.");
        comboBoxPriority.addItemListener(new java.awt.event.ItemListener() {
            public void itemStateChanged(java.awt.event.ItemEvent evt) {
                changePriority(evt);
            }
        });

        labelMaxCPUUsage.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelMaxCPUUsage.setText("Max CPU Usage:");

        spinnerMaxCPUUsage.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerMaxCPUUsage.setModel(new javax.swing.SpinnerNumberModel(1, 1, 100, 1));
        spinnerMaxCPUUsage.setToolTipText("specifies the maximum allowed CPU usage of a Runtime process and its childrens process during this plan.");
        spinnerMaxCPUUsage.addChangeListener(new javax.swing.event.ChangeListener() {
            public void stateChanged(javax.swing.event.ChangeEvent evt) {
                changeMaxCpuUsage(evt);
            }
        });

        labelForCent.setText("%");

        labelClassData.setFont(new java.awt.Font("TakaoPGothic", 0, 11));
        labelClassData.setText("ClassData:");

        spinnerClassData.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        spinnerClassData.setModel(new javax.swing.SpinnerNumberModel(1, 1, 7, 1));
        spinnerClassData.setToolTipText("specifies the classData of the Runtimes process and its chlidren processes during this plan.");

        org.jdesktop.layout.GroupLayout panelProcessManagementLayout = new org.jdesktop.layout.GroupLayout(panelProcessManagement);
        panelProcessManagement.setLayout(panelProcessManagementLayout);
        panelProcessManagementLayout.setHorizontalGroup(
            panelProcessManagementLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelProcessManagementLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelProcessManagementLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, labelClassData)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, labelProcessPrioriry))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelProcessManagementLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING, false)
                    .add(spinnerClassData)
                    .add(comboBoxPriority, 0, 115, Short.MAX_VALUE))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 96, Short.MAX_VALUE)
                .add(labelMaxCPUUsage)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.UNRELATED)
                .add(spinnerMaxCPUUsage, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 56, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.UNRELATED)
                .add(labelForCent)
                .addContainerGap())
        );
        panelProcessManagementLayout.setVerticalGroup(
            panelProcessManagementLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(panelProcessManagementLayout.createSequentialGroup()
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                .add(panelProcessManagementLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelProcessPrioriry)
                    .add(comboBoxPriority, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(spinnerMaxCPUUsage, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                    .add(labelForCent)
                    .add(labelMaxCPUUsage))
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(panelProcessManagementLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(labelClassData)
                    .add(spinnerClassData, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE))
                .addContainerGap())
        );

        checkBoxAlwaysAvailable.setFont(new java.awt.Font("TakaoPGothic", 0, 11)); // NOI18N
        checkBoxAlwaysAvailable.setText("Always available");
        checkBoxAlwaysAvailable.setToolTipText("no weekly planning, the ProActive Agent will be always available.");
        checkBoxAlwaysAvailable.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                alwaysAvailableChooser(evt);
            }
        });

        org.jdesktop.layout.GroupLayout PanelPlanningLayout = new org.jdesktop.layout.GroupLayout(PanelPlanning);
        PanelPlanning.setLayout(PanelPlanningLayout);
        PanelPlanningLayout.setHorizontalGroup(
            PanelPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, PanelPlanningLayout.createSequentialGroup()
                .addContainerGap()
                .add(panelWeeklyPlanning, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(PanelPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING, false)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, checkBoxAlwaysAvailable)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelProcessManagement, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, panelPlanEditor, 0, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
                .addContainerGap())
        );
        PanelPlanningLayout.setVerticalGroup(
            PanelPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(org.jdesktop.layout.GroupLayout.TRAILING, PanelPlanningLayout.createSequentialGroup()
                .addContainerGap()
                .add(PanelPlanningLayout.createParallelGroup(org.jdesktop.layout.GroupLayout.TRAILING)
                    .add(org.jdesktop.layout.GroupLayout.LEADING, panelWeeklyPlanning, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .add(PanelPlanningLayout.createSequentialGroup()
                        .add(panelPlanEditor, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(panelProcessManagement, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED, 111, Short.MAX_VALUE)
                        .add(checkBoxAlwaysAvailable)))
                .addContainerGap())
        );

        GlobalPanel.addTab("Planning", PanelPlanning);

        org.jdesktop.layout.GroupLayout layout = new org.jdesktop.layout.GroupLayout(getContentPane());
        getContentPane().setLayout(layout);
        layout.setHorizontalGroup(
            layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(layout.createSequentialGroup()
                .addContainerGap()
                .add(layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, GlobalPanel, org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, 828, Short.MAX_VALUE)
                    .add(org.jdesktop.layout.GroupLayout.TRAILING, layout.createSequentialGroup()
                        .add(ButtonSaveAs)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(ButtonSave)
                        .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                        .add(ButtonClose)))
                .addContainerGap())
        );
        layout.setVerticalGroup(
            layout.createParallelGroup(org.jdesktop.layout.GroupLayout.LEADING)
            .add(layout.createSequentialGroup()
                .addContainerGap()
                .add(GlobalPanel, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE, 504, org.jdesktop.layout.GroupLayout.PREFERRED_SIZE)
                .addPreferredGap(org.jdesktop.layout.LayoutStyle.RELATED)
                .add(layout.createParallelGroup(org.jdesktop.layout.GroupLayout.BASELINE)
                    .add(ButtonClose)
                    .add(ButtonSave)
                    .add(ButtonSaveAs))
                .addContainerGap(org.jdesktop.layout.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );

        pack();
    }// </editor-fold>//GEN-END:initComponents

    private void CPUsChooser(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_CPUsChooser
        if(checkBoxCPUs.isSelected()) {
            ModelManager.setNBRUNTIMES( Runtime.getRuntime().availableProcessors() ); 
            spinNbRuntimes.setEnabled(false);
        } else {
            ModelManager.setNBRUNTIMES( Integer.parseInt(spinNbRuntimes.getValue().toString()) );
            spinNbRuntimes.setEnabled(true);
            spinNbRuntimes.setValue(0);
        }
}//GEN-LAST:event_CPUsChooser

    /**
     * ProActive Home chooser()
     * @param evt 
     */
    private void browseProActiveHome(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_browseProActiveHome
        
        JFileChooser fc = new JFileChooser();
        fc.setCurrentDirectory(new java.io.File("."));
        fc.setDialogTitle("Choose your ProActive Home");
        fc.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
        fc.showOpenDialog(this);
        
        File selFile = fc.getSelectedFile();
       
        if(selFile != null) {
            textProActiveHome.setText(selFile.getAbsolutePath());
        }
        
    }//GEN-LAST:event_browseProActiveHome

    /**
     * JAVAHOME chooser()
     * @param evt 
     */
    private void browseJavaHome(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_browseJavaHome
       
        JFileChooser fc = new JFileChooser();
        fc.setCurrentDirectory(new java.io.File("."));
        fc.setDialogTitle("Choose your JAVAHOME");
        fc.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
        fc.showOpenDialog(this);
        
        File selFile = fc.getSelectedFile();
       
        if(selFile != null) {
            textJavaHome.setText(selFile.getAbsolutePath());
            ModelManager.setJAVAHOME(selFile.getAbsolutePath());
        }
    }//GEN-LAST:event_browseJavaHome

    /**
     * the checkBox chooser to let System get back information on JAVAHOME
     * @param evt 
     */
    private void JavaHomeSystemChooser(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_JavaHomeSystemChooser
        
        if(checkBoxJavaHome.isSelected()) {
            ModelManager.setJAVAHOME( System.getProperty("java.home") ); 
            buttonBrowseJavaHome.setEnabled(false);
            textJavaHome.setEnabled(false);
        } else {
            ModelManager.setJAVAHOME( textJavaHome.getText() );
            buttonBrowseJavaHome.setEnabled(true);
            textJavaHome.setEnabled(true);
        }
    }//GEN-LAST:event_JavaHomeSystemChooser

    /**
     * Script on exit file chooser()
     * @param evt 
     */
    private void browseScriptOnExit(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_browseScriptOnExit

        JFileChooser fc = new JFileChooser();
        fc.setCurrentDirectory(new java.io.File("."));
        fc.setDialogTitle("Choose your script");
        fc.showOpenDialog(this);
        
        File selFile = fc.getSelectedFile();
       
        if(selFile != null) {
            textScriptLocation.setText(selFile.getAbsolutePath());
        }
    }//GEN-LAST:event_browseScriptOnExit

    /**
     * The Close button
     * @param evt 
     */
    private void close(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_close
        this.setVisible(false);
    }//GEN-LAST:event_close

    /**
     * @return the datas from right panel in Event class 
     */
    private Event getEventFromPanel() {
        
        //Configurations
        Config conf = new Config();
        conf.setMemoryLimit( ModelManager.getMEMORYLIMIT() );
        conf.setProcessPriority( comboBoxPriority.getSelectedItem().toString() );
        conf.setCpuUsage(Integer.parseInt(spinnerMaxCPUUsage.getValue().toString()));
        conf.setPortRangeFirst(Integer.parseInt(spinPortInitialValue.getValue().toString()));
        conf.setPortRangeLast(MAX_RANGE_PORT);
        conf.setNbRuntimes(Integer.parseInt(spinNbRuntimes.getValue().toString()));
        
        //StartTime Planning
        PlanningTime startTime = new PlanningTime(
                    (String)comboBoxStartDay.getSelectedItem(),
                    Integer.parseInt(spinnerStartHours.getValue().toString()),
                    Integer.parseInt(spinnerStartMinutes.getValue().toString()),
                    Integer.parseInt(spinnerStartSecondes.getValue().toString())
                    );
        
        //Duration Planning
        Duration duration = new Duration(
                    Integer.parseInt(spinnerDurationDays.getValue().toString()),
                    Integer.parseInt(spinnerDurationHours.getValue().toString()),
                    Integer.parseInt(spinnerDurationMinutes.getValue().toString()),
                    Integer.parseInt(spinnerDurationSecondes.getValue().toString())
                    );
        
        //return event
        return new Event(startTime, duration, conf);
        
    }
    
    /**
     * Update right Panel with specific Event
     * @param ev 
     */
    private void setEventToPanel(Event ev) {
        
        LockPlanning = true;
        
        comboBoxStartDay.setSelectedIndex(
                ModelManager.convertDayToInt(
                    ev.getStartTime().getDay())
                );
        spinnerStartHours.setValue(ev.getStartTime().getHour());
        spinnerStartMinutes.setValue(ev.getStartTime().getMinute());
        spinnerStartSecondes.setValue(ev.getStartTime().getSecond());
        spinnerDurationDays.setValue(ev.getDuration().getDay());
        spinnerDurationHours.setValue(ev.getDuration().getHour());
        spinnerDurationMinutes.setValue(ev.getDuration().getMinute());
        spinnerDurationSecondes.setValue(ev.getDuration().getSecond());
        comboBoxPriority.setSelectedIndex(ModelManager.convertPriorityToInt(ev.getConfig().getProcessPriority()));
        spinnerMaxCPUUsage.setValue(ev.getConfig().getCpuUsage());
        spinNbRuntimes.setValue(ev.getConfig().getNbRuntimes());
        spinPortInitialValue.setValue(ev.getConfig().getPortRangeFirst());
        
        LockPlanning = false;
    }
    
    /**
     * Add an event to the JList view
     * @param ev 
     */
    private void addEventToPlanningTab(Event ev) {
        
        PlanningTime endTime = TimeComputer.addTime(
                ev.getStartTime(), ev.getDuration().getDay(),
                ev.getDuration().getHour(), ev.getDuration().getMinute() ,
                ev.getDuration().getSecond());
        
        DefaultListModel listModel = new DefaultListModel();  
        for(int i=0 ; i< listPlanning.getModel().getSize() ; i++ ){  
            listModel.addElement(listPlanning.getModel().getElementAt(i));  
        }          
        listModel.addElement( ev.getStartTime().toString() + " => " + endTime.toString());  
        listPlanning.setModel(listModel);  
        
    }
    
    /**
     * modify the selected event on the JList view
     */
    public void setEventToPlanningTab() {
        
        int idSelected = listPlanning.getSelectedIndex();
        
        if(idSelected != -1 && !LockPlanning ) {
            
            //PLANNING MODIFS...
            Event ev = getEventFromPanel();
            ModelManager.getEVENTS().setEventById(idSelected, ev);
            
            PlanningTime endTime = TimeComputer.addTime(
                ev.getStartTime(), ev.getDuration().getDay(),
                ev.getDuration().getHour(), ev.getDuration().getMinute() ,
                ev.getDuration().getSecond());
            
            DefaultListModel listModel = new DefaultListModel();  
            for(int i=0 ; i< listPlanning.getModel().getSize() ; i++ ){
                listModel.addElement(listPlanning.getModel().getElementAt(i));  
            }
            
            listModel.setElementAt(ev.getStartTime().toString() + " => " + endTime.toString(), idSelected); 
            listPlanning.setModel(listModel);  
            listPlanning.clearSelection();
            listPlanning.setSelectionInterval(idSelected, idSelected);
            
        } 
    }
    
    private void saveConnections() {
        
        //LOCAL CONNECTION
         ModelManager.getCONNECTIONS().getLocal().setJavaStarterClass(textStarterClass4.getText());
         ModelManager.getCONNECTIONS().getLocal().setNodeName(textNodeName1.getText());
         ModelManager.getCONNECTIONS().getLocal().setRespawnIncrement(10);
        //RM CONNECTION
         ModelManager.getCONNECTIONS().getRMConn().setJavaStarterClass(textStarterClass2.getText());
         ModelManager.getCONNECTIONS().getRMConn().setNodeName(textNodeName2.getText());
         ModelManager.getCONNECTIONS().getRMConn().setRespawnIncrement(10);
         ModelManager.getCONNECTIONS().getRMConn().setUrl(textRMURL.getText());
         ModelManager.getCONNECTIONS().getRMConn().setNodeSourceName(textNodeSourceName.getText());
         ModelManager.getCONNECTIONS().getRMConn().setCredential(textCredential.getText());
        //CUSTOM CONNECTION
         ModelManager.getCONNECTIONS().getCustom().setJavaStarterClass(textStarterClass3.getText());
         ModelManager.getCONNECTIONS().getCustom().setNodeName("?");
         ModelManager.getCONNECTIONS().getCustom().setRespawnIncrement(10);
        
        if(radioLocal.isSelected()) {
             ModelManager.getCONNECTIONS().setSelected(CONNTYPE.LOCAL);
        }
        else if(radioResourceManager.isSelected()) {
             ModelManager.getCONNECTIONS().setSelected(CONNTYPE.RM);
        }
        else if(radioCustom.isSelected()) {
             ModelManager.getCONNECTIONS().setSelected(CONNTYPE.CUSTOM);
        }
    }
    
    /**
     * get back the JLabel informations on the Model
     */
    private void save() {
        
        for (Event ev : ModelManager.getEVENTS().getListEvents()) {
            
            ev.getConfig().setProActiveHome(textProActiveHome.getText());
            ev.getConfig().setJavaHome(textJavaHome.getText());
            ev.getConfig().setProtocol(comboBoxProtocol.getSelectedItem().toString());
            //NB Runtimes
            if(Integer.parseInt(spinNbRuntimes.getValue().toString())==0)
            {
                ev.getConfig().setNbRuntimes(Runtime.getRuntime().availableProcessors());
                ModelManager.setNBRUNTIMES(Runtime.getRuntime().availableProcessors());
            } else {
                ev.getConfig().setNbRuntimes(Integer.parseInt(spinNbRuntimes.getValue().toString()));
                ModelManager.setNBRUNTIMES(Integer.parseInt(spinNbRuntimes.getValue().toString()));
            }
        }
        
        saveConnections();
        
        ModelManager.setSCRIPTONEXIT( textScriptLocation.getText() );
        ModelManager.setJAVAHOME(textJavaHome.getText());
        ModelManager.setPROACTIVEHOME(textProActiveHome.getText());
        ModelManager.setPROTOCOL(comboBoxProtocol.getSelectedItem().toString());
        if(ModelManager.getEVENTS().getLastEvent() != null) {
            
            ModelManager.setPROCESSPRIORITY(
                ModelManager.getEVENTS().getLastEvent().getConfig().getProcessPriority());
            ModelManager.setCPUUSAGE(
                ModelManager.getEVENTS().getLastEvent().getConfig().getCpuUsage());
            ModelManager.setPORTMIN(
                    ModelManager.getEVENTS().getLastEvent().getConfig().getPortRangeFirst());
            ModelManager.setPORTMAX(
                    ModelManager.getEVENTS().getLastEvent().getConfig().getPortRangeLast());
        }
    }
    
    /**
     * The Save methode put all Model informations on the XML config file
     * @param evt 
     */
    private void saveProperties(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_saveProperties
        save();
        
        if(ModelManager.saveXML(this)) {
            System.out.println("save : " + ModelManager.getXMLFileName() + " [OK] ");
        } else {
            System.out.println("save : " + ModelManager.getXMLFileName() + " [FAIL] ");
        }
        
    }//GEN-LAST:event_saveProperties

    /**
     * The Save methode put all Model informations on the specific XML config file
     * @param evt 
     */
    private void saveAsProperties(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_saveAsProperties
        
        FileFilter filter = new ExtensionFileFilter("XML Files", new String[] { "XML", "xml" });
        
        JFileChooser fc = new JFileChooser();
        fc.setFileFilter(filter);
        fc.setCurrentDirectory(new java.io.File("."));
        fc.setDialogTitle("Select your properties file");
        int actionDialog = fc.showSaveDialog(this);
        
        File selFile = fc.getSelectedFile();
       
        if(selFile != null) {
            
            if(selFile.exists())
            {
                actionDialog = JOptionPane.showConfirmDialog(this, "Replace existing file?");
                if(actionDialog == JOptionPane.YES_OPTION)
                {
                    save();
                    if(ModelManager.saveXML(this,selFile.getAbsolutePath())) {
                        System.out.println("saveAs : " + selFile.getAbsolutePath() + " [OK] ");
                    } else {
                        System.out.println("save : " + selFile.getAbsolutePath() + " [FAIL] ");
                    }
                }
            } else {
                save();
                if(ModelManager.saveXML(this,selFile.getAbsolutePath())) {
                    System.out.println("saveAs : " + selFile.getAbsolutePath() + " [OK] ");
                } else {
                    System.out.println("save : " + selFile.getAbsolutePath() + " [FAIL] ");
                }
            }
        }
    }//GEN-LAST:event_saveAsProperties

    /**
     * Update Model information with the new SpinMemory value
     * @param evt 
     */
    private void listenSpinMemory(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_listenSpinMemory
        
        // Get the new value
        ModelManager.setMEMORYLIMIT( Integer.parseInt(splinMemoryLimit.getValue().toString()) );
    }//GEN-LAST:event_listenSpinMemory

    /**
     * The CreatePlan button, update JList view and Model
     * @param evt 
     */
    private void createPlan(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_createPlan
        
        Event newEvent = getEventFromPanel();
        addEventToPlanningTab(newEvent);
        
        ModelManager.getEVENTS().addEvent(newEvent);
    }//GEN-LAST:event_createPlan

    /**
     * The Delete button, update JList view and Model
     * @param evt 
     */
    private void deletePlan(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_deletePlan
        
        if(listPlanning.getSelectedIndex() != -1 ) {
            DefaultListModel listModel = new DefaultListModel();  
            for(int i=0 ; i< listPlanning.getModel().getSize() ; i++ ){  
                listModel.addElement(listPlanning.getModel().getElementAt(i));  
            }          
            
            int idToRemove = listPlanning.getSelectedIndex();
            listModel.remove(idToRemove);
            listPlanning.setModel(listModel); 
            
            ModelManager.getEVENTS().removeEventById(idToRemove);
            
            if(ModelManager.getEVENTS().getSize() > 0) {
                listPlanning.clearSelection();
                listPlanning.setSelectionInterval(0, 0);
            }
        }
    }//GEN-LAST:event_deletePlan

    /**
     * The selection method when user choose a specific event in planning tab
     * @param evt 
     */
    private void SelectElement(javax.swing.event.ListSelectionEvent evt) {//GEN-FIRST:event_SelectElement

        if(listPlanning.getSelectedIndex() != -1 ) {
            Event ev = ModelManager.getEVENTS().get(listPlanning.getSelectedIndex());
            setEventToPanel(ev);
        }
    }//GEN-LAST:event_SelectElement

    /**
     * Some listenners to dynamical update of JList view 
     * @param evt 
     */
    
    private void changeDay(java.awt.event.ItemEvent evt) {//GEN-FIRST:event_changeDay
        setEventToPlanningTab();
    }//GEN-LAST:event_changeDay

    private void changeHour(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeHour
        setEventToPlanningTab();
    }//GEN-LAST:event_changeHour

    private void changeMinutes(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeMinutes
        setEventToPlanningTab();
    }//GEN-LAST:event_changeMinutes

    private void changeSeconds(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeSeconds
        setEventToPlanningTab();
    }//GEN-LAST:event_changeSeconds

    private void changeDurationDay(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeDurationDay
        setEventToPlanningTab();
    }//GEN-LAST:event_changeDurationDay

    private void changeDurationHour(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeDurationHour
        setEventToPlanningTab();
    }//GEN-LAST:event_changeDurationHour

    private void changeDurationMinutes(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeDurationMinutes
       setEventToPlanningTab();
    }//GEN-LAST:event_changeDurationMinutes

    private void changeDurationSecond(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeDurationSecond
        setEventToPlanningTab();
    }//GEN-LAST:event_changeDurationSecond

    private void changePriority(java.awt.event.ItemEvent evt) {//GEN-FIRST:event_changePriority
        setEventToPlanningTab();
    }//GEN-LAST:event_changePriority

    private void changeMaxCpuUsage(javax.swing.event.ChangeEvent evt) {//GEN-FIRST:event_changeMaxCpuUsage
        setEventToPlanningTab();
    }//GEN-LAST:event_changeMaxCpuUsage

    private void alwaysAvailableChooser(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_alwaysAvailableChooser
        
        if(checkBoxAlwaysAvailable.isSelected()) {
            
            String text = "Always available will remove all plans, do you want to save your current planning ?";
            int answer = JOptionPane.showConfirmDialog(null, text, "Input",
                                      JOptionPane.YES_NO_CANCEL_OPTION);
            
            if(answer == JOptionPane.YES_OPTION) {
                
                saveAsProperties(evt);
                
            }
            if(answer == JOptionPane.YES_OPTION || answer == JOptionPane.NO_OPTION) {
                ModelManager.getEVENTS().freeEventsList();
                DefaultListModel listModel = new DefaultListModel();
                listPlanning.setModel(listModel); 

                spinnerDurationDays.setEnabled(false);
                spinnerDurationHours.setEnabled(false);
                spinnerDurationMinutes.setEnabled(false);
                spinnerDurationSecondes.setEnabled(false);
                comboBoxStartDay.setEnabled(false);
                spinnerStartHours.setEnabled(false);
                spinnerStartMinutes.setEnabled(false);
                spinnerStartSecondes.setEnabled(false);
                
                buttonCreatePlan.setEnabled(false);
                buttonDeletePlan.setEnabled(false);
            } else {
                checkBoxAlwaysAvailable.setSelected(false);
            }
        } else {
            spinnerDurationDays.setEnabled(true);
            spinnerDurationHours.setEnabled(true);
            spinnerDurationMinutes.setEnabled(true);
            spinnerDurationSecondes.setEnabled(true);
            comboBoxStartDay.setEnabled(true);
            spinnerStartHours.setEnabled(true);
            spinnerStartMinutes.setEnabled(true);
            spinnerStartSecondes.setEnabled(true);
            
            buttonCreatePlan.setEnabled(true);
            buttonDeletePlan.setEnabled(true);
        }
    }//GEN-LAST:event_alwaysAvailableChooser

    private void refreshInterfacesList(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_refreshInterfacesList
        
        ModelManager.setListInterfaces(ListNetworkInterfaces.getNetworkInterfacesList());
        DefaultListModel listModel = new DefaultListModel(); 
        
        for (String s : ModelManager.getListInterfaces()) {
            listModel.addElement(s);
        }
        
        JListInterfaces.setModel(listModel);
        
        buttonUse.setEnabled(true);
    }//GEN-LAST:event_refreshInterfacesList

    /**
     * 
     * @param Option
     * @return if the element has been added or not 
     */
    private int setOptionToJVMOptionList(String net) {
        
        String interfaceModel = "-Dproactive.net.interface=";
        if(net.toString().length() > 26) {
            if(net.toString().substring(0, 26)
                        .equals( interfaceModel )) {
                return setNetWorkInterfaceToJVMOptionList(net);
            }
        } 
        
        DefaultListModel listModel = new DefaultListModel();  
        for(int i=0 ; i< JListJVMOption.getModel().getSize() ; i++ ){  
            listModel.addElement(JListJVMOption.getModel().getElementAt(i));
        }  

        listModel.addElement(net);
        JListJVMOption.setModel(listModel);

        return -1;
    }
    
    /**
     * 
     * @param net
     * @return if the element has been added or not 
     */
    private int setNetWorkInterfaceToJVMOptionList(String net) {
        
        int rank = -1;
        String interfaceModel = "-Dproactive.net.interface=";

        DefaultListModel listModel = new DefaultListModel();  
        for(int i=0 ; i< JListJVMOption.getModel().getSize() ; i++ ){  
            listModel.addElement(JListJVMOption.getModel().getElementAt(i));
            
            //Test for Check the Network Interface Options :
            if(listModel.getElementAt(i).toString().length() > 26) {
                if(listModel.getElementAt(i).toString().substring(0, 26)
                        .equals( interfaceModel )) {

                    //if a network interface already exist
                    listModel.setElementAt(net, i);
                    rank = i;
                }
            }
        }  
        if(rank == -1) {
            listModel.addElement(net);
        }
        JListJVMOption.setModel(listModel);

        return rank;
    }
    
    private void useInterface(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_useInterface
       
        int index;
        if((index = JListInterfaces.getSelectedIndex()) != -1) {
            
            String net = ModelManager.getListInterfaces().get(index);
            net = net.substring(0, net.indexOf(" "));
            net = "-Dproactive.net.interface=" + net;
            
            int rank = setOptionToJVMOptionList(net);
            if( rank == -1) {
                ModelManager.getJVMOPTIONS().add(net);
            } else {
                ModelManager.getJVMOPTIONS().set(rank, net);
            }
        }
    }//GEN-LAST:event_useInterface

    private void removeJVMOption(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_removeJVMOption
        int index;
        if((index = JListJVMOption.getSelectedIndex()) != -1) {
            
            DefaultListModel listModel = new DefaultListModel();  
            for(int i=0 ; i< JListJVMOption.getModel().getSize() ; i++ ){  
                listModel.addElement(JListJVMOption.getModel().getElementAt(i));  
            }  
            listModel.remove(index);
            ModelManager.getJVMOPTIONS().remove(index);
            
            JListJVMOption.setModel(listModel);
        }
    }//GEN-LAST:event_removeJVMOption

    private void selectJVMOption(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_selectJVMOption
        
        int index;
        // EDIT MODE
        if(evt.getClickCount() == 2) {
            if((index = JListJVMOption.getSelectedIndex()) != -1) {
            
                String message = "New value : ";
                String newValue = JOptionPane.showInputDialog(
                        message,JListJVMOption.getSelectedValue() );
                
                if(newValue != null) {
                
                    DefaultListModel listModel = new DefaultListModel();  
                    for(int i=0 ; i< JListJVMOption.getModel().getSize() ; i++ ){  
                        listModel.addElement(JListJVMOption.getModel().getElementAt(i));  
                    }  
                    listModel.set(index,newValue);
                    ModelManager.getJVMOPTIONS().set(index, newValue);
                    
                    JListJVMOption.setModel(listModel);
                }
            }
        }
    }//GEN-LAST:event_selectJVMOption

    private void selectNetworkInterface(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_selectNetworkInterface
        int index;
        // EDIT MODE
        if(evt.getClickCount() == 2) {
            if((index = JListInterfaces.getSelectedIndex()) != -1) {
            
                String net = ModelManager.getListInterfaces().get(index);
                net = net.substring(0, net.indexOf(" "));
                net = "-Dproactive.net.interface=" + net;

                int rank = setOptionToJVMOptionList(net);
                if( rank == -1) {
                    ModelManager.getJVMOPTIONS().add(net);
                } else {
                    ModelManager.getJVMOPTIONS().set(rank, net);
                }
            }
        }
    }//GEN-LAST:event_selectNetworkInterface

    private void addJVMOption(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_addJVMOption

        String newOption = "New option";
        setOptionToJVMOptionList(newOption);
        ModelManager.getJVMOPTIONS().add(newOption);
    }//GEN-LAST:event_addJVMOption

    private void setArgumentToTheArgumentList(String newArgument) {
        
        DefaultListModel listModel = new DefaultListModel();  
        for(int i=0 ; i< JlistArguments.getModel().getSize() ; i++ ){  
            listModel.addElement(JlistArguments.getModel().getElementAt(i));
        }  

        listModel.addElement(newArgument);
        JlistArguments.setModel(listModel);
    }
    
    private void addArgumentToCustomConnexion(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_addArgumentToCustomConnexion
        
        String newArgument;
        if(textArgument.getText().equals("")) {
            newArgument = "-New argument";
        } else {
            newArgument = textArgument.getText();
        }
        setArgumentToTheArgumentList(newArgument);
        ModelManager.getCONNECTIONS().getCustom().addArgs(newArgument);
    }//GEN-LAST:event_addArgumentToCustomConnexion

    private void removeArgument(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_removeArgument
        int index;
        if((index = JlistArguments.getSelectedIndex()) != -1) {
            
            DefaultListModel listModel = new DefaultListModel();  
            for(int i=0 ; i< JlistArguments.getModel().getSize() ; i++ ){  
                listModel.addElement(JlistArguments.getModel().getElementAt(i));  
            }  
            listModel.remove(index);
            ModelManager.getCONNECTIONS().getCustom().removeArg(index);
            
            JlistArguments.setModel(listModel);
        }
    }//GEN-LAST:event_removeArgument

    private void selectArgument(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_selectArgument
        int index;
        // EDIT MODE
        if(evt.getClickCount() == 2) {
            if((index = JlistArguments.getSelectedIndex()) != -1) {
            
                String message = "New value : ";
                String newValue = JOptionPane.showInputDialog(
                        message,JlistArguments.getSelectedValue() );
                
                if(newValue != null) {
                
                    DefaultListModel listModel = new DefaultListModel();  
                    for(int i=0 ; i< JlistArguments.getModel().getSize() ; i++ ){  
                        listModel.addElement(JlistArguments.getModel().getElementAt(i));  
                    }  
                    listModel.set(index,newValue);
                    ModelManager.getCONNECTIONS().getCustom().setArg(index, newValue);
                    
                    JlistArguments.setModel(listModel);
                }
            }
        }
    }//GEN-LAST:event_selectArgument

    private void browseCredential(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_browseCredential
        FileFilter filter = new ExtensionFileFilter("Credentials .cred", new String[] { "CRED", "cred" });
        
        JFileChooser fc = new JFileChooser();
        fc.setFileFilter(filter);
        fc.setCurrentDirectory(new java.io.File("."));
        fc.setDialogTitle("Select your properties file");
        int actionDialog = fc.showSaveDialog(this);
        
        File selFile = fc.getSelectedFile();
        
        if(selFile != null) {
            textCredential.setText(selFile.getAbsolutePath());
            ModelManager.getCONNECTIONS().getRMConn().setCredential(selFile.getAbsolutePath());
        }
    }//GEN-LAST:event_browseCredential

    private void showGraphic(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_showGraphic
        
        showApplet.setEvList(ModelManager.getEVENTS());
        
        final JFrame f = new JFrame("Planning View");
        f.addWindowListener(new WindowAdapter() {
          public void windowClosing(WindowEvent e) {
            f.setVisible(false);
          }
        });
        f.getContentPane().add("Center", showApplet);
        f.pack();
        f.setSize(new Dimension(350, 350));
        f.show();
    }//GEN-LAST:event_showGraphic

    private void selectLocalPanel(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_selectLocalPanel
        panelMultipleConnections.setSelectedIndex(0);
}//GEN-LAST:event_selectLocalPanel

    private void selectRMPanel(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_selectRMPanel
        panelMultipleConnections.setSelectedIndex(1);
    }//GEN-LAST:event_selectRMPanel

    private void selectCustomPanel(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_selectCustomPanel
        panelMultipleConnections.setSelectedIndex(2);
    }//GEN-LAST:event_selectCustomPanel

    /**
     * @param args the command line arguments
     */
    public static void main(String args[]) {
        java.awt.EventQueue.invokeLater(new Runnable() {

            public void run() {
                GUIEditorWindows dialog = new GUIEditorWindows();
                dialog.addWindowListener(new java.awt.event.WindowAdapter() {

                    public void windowClosing(java.awt.event.WindowEvent e) {
                        System.exit(0);
                    }
                });
                dialog.setVisible(true);
            }
        });
    }
    
    private final String TITLE_MAIN_FRAME = "GUI Editor";
    private Boolean LockPlanning = false;
    private int MAX_RANGE_PORT = 65534;
    private ShowApplet showApplet = new ShowApplet();
    private String icon = "icon.JPG";
    
    // Variables declaration - do not modify//GEN-BEGIN:variables
    private javax.swing.JButton ButtonClose;
    private javax.swing.JButton ButtonSave;
    private javax.swing.JButton ButtonSaveAs;
    private javax.swing.JTabbedPane GlobalPanel;
    private javax.swing.JList JListInterfaces;
    private javax.swing.JList JListJVMOption;
    private javax.swing.JList JlistArguments;
    private javax.swing.JPanel PanelANI;
    private javax.swing.JPanel PanelAdditionnalConf2;
    private javax.swing.JPanel PanelAdditionnalConf3;
    private javax.swing.JPanel PanelAdditionnalConf4;
    private javax.swing.JPanel PanelConfig;
    private javax.swing.JPanel PanelConnection;
    private javax.swing.JPanel PanelGeneral;
    private javax.swing.JPanel PanelMR;
    private javax.swing.JPanel PanelORE;
    private javax.swing.JPanel PanelPlanning;
    private javax.swing.JPanel PanelRML;
    private javax.swing.JButton buttonAdd;
    private javax.swing.JButton buttonAddJVMOpt;
    private javax.swing.JButton buttonBrowseJavaHome;
    private javax.swing.JButton buttonBrowseLocation;
    private javax.swing.JButton buttonBrowseProActiveHome;
    private javax.swing.JButton buttonBrowseScriptLocation;
    private javax.swing.JButton buttonCreatePlan;
    private javax.swing.JButton buttonDelete;
    private javax.swing.JButton buttonDeletePlan;
    private javax.swing.ButtonGroup buttonGroup1;
    private javax.swing.JButton buttonRefresh;
    private javax.swing.JButton buttonRemoveJVMOpt;
    private javax.swing.JButton buttonSaveArg;
    private javax.swing.JButton buttonShowPlan;
    private javax.swing.JButton buttonUse;
    private javax.swing.JCheckBox checkBoxAlwaysAvailable;
    private javax.swing.JCheckBox checkBoxCPUs;
    private javax.swing.JCheckBox checkBoxJavaHome;
    private javax.swing.JComboBox comboBoxPriority;
    private javax.swing.JComboBox comboBoxProtocol;
    private javax.swing.JComboBox comboBoxStartDay;
    private javax.swing.JList jList1;
    private javax.swing.JScrollPane jScrollPane1;
    private javax.swing.JScrollPane jScrollPane2;
    private javax.swing.JScrollPane jScrollPane3;
    private javax.swing.JScrollPane jScrollPane4;
    private javax.swing.JLabel labelArgument;
    private javax.swing.JLabel labelArguments;
    private javax.swing.JLabel labelCPUs;
    private javax.swing.JLabel labelClassData;
    private javax.swing.JLabel labelDurationDays;
    private javax.swing.JLabel labelDurationHours;
    private javax.swing.JLabel labelDurationMinutes;
    private javax.swing.JLabel labelDurationSecondes;
    private javax.swing.JLabel labelForCent;
    private javax.swing.JLabel labelMaxCPUUsage;
    private javax.swing.JLabel labelMemory;
    private javax.swing.JLabel labelMemoryLimit;
    private javax.swing.JLabel labelNbRuntimes;
    private javax.swing.JLabel labelNodeName1;
    private javax.swing.JLabel labelNodeName2;
    private javax.swing.JLabel labelNodeSourceName;
    private javax.swing.JLabel labelPortInitialValue1;
    private javax.swing.JLabel labelProcessPrioriry;
    private javax.swing.JLabel labelProtocol1;
    private javax.swing.JLabel labelRMURL;
    private javax.swing.JLabel labelStartDay;
    private javax.swing.JLabel labelStartHours;
    private javax.swing.JLabel labelStartMinutes;
    private javax.swing.JLabel labelStartSecondes;
    private javax.swing.JLabel labelStarterClass2;
    private javax.swing.JLabel labelStarterClass3;
    private javax.swing.JLabel labelStarterClass4;
    private javax.swing.JList listPlanning;
    private javax.swing.JLabel notice1;
    private javax.swing.JLabel notice2;
    private javax.swing.JPanel panelAuthCredential;
    private javax.swing.JPanel panelCustom;
    private javax.swing.JPanel panelCustom2;
    private javax.swing.JPanel panelDuration;
    private javax.swing.JPanel panelLocal;
    private javax.swing.JPanel panelLocalRegistration;
    private javax.swing.JTabbedPane panelMultipleConnections;
    private javax.swing.JPanel panelPlanEditor;
    private javax.swing.JPanel panelProcessManagement;
    private javax.swing.JPanel panelRMRegistration;
    private javax.swing.JPanel panelResourceManager;
    private javax.swing.JPanel panelRuntimeIncomingProtocol1;
    private javax.swing.JPanel panelStartTime;
    private javax.swing.JPanel panelWeeklyPlanning;
    private javax.swing.JRadioButton radioCustom;
    private javax.swing.JRadioButton radioLocal;
    private javax.swing.JRadioButton radioResourceManager;
    private javax.swing.JSpinner spinNbRuntimes;
    private javax.swing.JSpinner spinPortInitialValue;
    private javax.swing.JSpinner spinnerClassData;
    private javax.swing.JSpinner spinnerDurationDays;
    private javax.swing.JSpinner spinnerDurationHours;
    private javax.swing.JSpinner spinnerDurationMinutes;
    private javax.swing.JSpinner spinnerDurationSecondes;
    private javax.swing.JSpinner spinnerMaxCPUUsage;
    private javax.swing.JSpinner spinnerStartHours;
    private javax.swing.JSpinner spinnerStartMinutes;
    private javax.swing.JSpinner spinnerStartSecondes;
    private javax.swing.JSpinner splinMemoryLimit;
    private javax.swing.JScrollPane testAeraPlanning;
    private javax.swing.JTextField textArgument;
    private javax.swing.JLabel textCPUs;
    private javax.swing.JTextField textCredential;
    private javax.swing.JTextField textJavaHome;
    private javax.swing.JLabel textMemory;
    private javax.swing.JTextField textNodeName1;
    private javax.swing.JTextField textNodeName2;
    private javax.swing.JTextField textNodeSourceName;
    private javax.swing.JTextField textProActiveHome;
    private javax.swing.JTextField textRMURL;
    private javax.swing.JTextField textScriptLocation;
    private javax.swing.JTextField textStarterClass2;
    private javax.swing.JTextField textStarterClass3;
    private javax.swing.JTextField textStarterClass4;
    // End of variables declaration//GEN-END:variables

}
