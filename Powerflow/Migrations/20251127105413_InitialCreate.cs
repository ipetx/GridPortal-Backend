using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Powerflow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseSet",
                columns: table => new
                {
                    CaseSetNum = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CaseSetYear = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseSet", x => x.CaseSetNum);
                });

            migrationBuilder.CreateTable(
                name: "CaseInfo",
                columns: table => new
                {
                    CaseNum = table.Column<int>(type: "integer", nullable: false),
                    DOR = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CaseYear = table.Column<int>(type: "integer", nullable: true),
                    CaseSeason = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CaseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    CaseSetNum = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseInfo", x => x.CaseNum);
                    table.ForeignKey(
                        name: "FK_CaseInfo_CaseSet_CaseSetNum",
                        column: x => x.CaseSetNum,
                        principalTable: "CaseSet",
                        principalColumn: "CaseSetNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Adjust",
                columns: table => new
                {
                    acctap = table.Column<double>(type: "double precision", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    adjthr = table.Column<double>(type: "double precision", nullable: true),
                    mxswim = table.Column<int>(type: "integer", nullable: true),
                    mxtpss = table.Column<int>(type: "integer", nullable: true),
                    swvbnd = table.Column<double>(type: "double precision", nullable: true),
                    taplim = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjust", x => new { x.acctap, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Adjust_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    iarea = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    arname = table.Column<string>(type: "text", nullable: true),
                    isw = table.Column<int>(type: "integer", nullable: true),
                    pdes = table.Column<double>(type: "double precision", nullable: true),
                    ptol = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => new { x.iarea, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Area_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Caseid",
                columns: table => new
                {
                    ic = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    basfrq = table.Column<double>(type: "double precision", nullable: true),
                    nxfrat = table.Column<int>(type: "integer", nullable: true),
                    rev = table.Column<int>(type: "integer", nullable: true),
                    sbase = table.Column<double>(type: "double precision", nullable: true),
                    title1 = table.Column<string>(type: "text", nullable: true),
                    title2 = table.Column<string>(type: "text", nullable: true),
                    xfrrat = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caseid", x => new { x.ic, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Caseid_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gauss",
                columns: table => new
                {
                    itmx = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    accm = table.Column<double>(type: "double precision", nullable: true),
                    accp = table.Column<double>(type: "double precision", nullable: true),
                    accq = table.Column<double>(type: "double precision", nullable: true),
                    tol = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gauss", x => new { x.itmx, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Gauss_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "General",
                columns: table => new
                {
                    blowup = table.Column<double>(type: "double precision", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    camaxreptsln = table.Column<int>(type: "integer", nullable: true),
                    chkdupcntlbl = table.Column<int>(type: "integer", nullable: true),
                    maxisollvls = table.Column<int>(type: "integer", nullable: true),
                    pqbrak = table.Column<double>(type: "double precision", nullable: true),
                    thrshz = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_General", x => new { x.blowup, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_General_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Impcor",
                columns: table => new
                {
                    itable = table.Column<int>(type: "integer", nullable: false),
                    tap = table.Column<double>(type: "double precision", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    imfact = table.Column<double>(type: "double precision", nullable: true),
                    refact = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impcor", x => new { x.itable, x.tap, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Impcor_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Newton",
                columns: table => new
                {
                    itmxn = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    accn = table.Column<double>(type: "double precision", nullable: true),
                    dvlim = table.Column<double>(type: "double precision", nullable: true),
                    ndvfct = table.Column<double>(type: "double precision", nullable: true),
                    toln = table.Column<double>(type: "double precision", nullable: true),
                    vctolq = table.Column<double>(type: "double precision", nullable: true),
                    vctolv = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newton", x => new { x.itmxn, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Newton_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ntermdc",
                columns: table => new
                {
                    vconv = table.Column<int>(type: "integer", nullable: false),
                    vconvn = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    mdc = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    nconv = table.Column<int>(type: "integer", nullable: true),
                    ndcbs = table.Column<int>(type: "integer", nullable: true),
                    ndcln = table.Column<int>(type: "integer", nullable: true),
                    vcmod = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ntermdc", x => new { x.vconv, x.vconvn, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Ntermdc_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ntermdcconv",
                columns: table => new
                {
                    ib = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    angmn = table.Column<double>(type: "double precision", nullable: true),
                    angmx = table.Column<double>(type: "double precision", nullable: true),
                    cnvcod = table.Column<int>(type: "integer", nullable: true),
                    dcpf = table.Column<double>(type: "double precision", nullable: true),
                    ebas = table.Column<double>(type: "double precision", nullable: true),
                    marg = table.Column<double>(type: "double precision", nullable: true),
                    n = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    rc = table.Column<double>(type: "double precision", nullable: true),
                    setvl = table.Column<double>(type: "double precision", nullable: true),
                    tap = table.Column<double>(type: "double precision", nullable: true),
                    tpmn = table.Column<double>(type: "double precision", nullable: true),
                    tpmx = table.Column<double>(type: "double precision", nullable: true),
                    tr = table.Column<double>(type: "double precision", nullable: true),
                    tstp = table.Column<double>(type: "double precision", nullable: true),
                    xc = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ntermdcconv", x => new { x.ib, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Ntermdcconv_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ntermdclink",
                columns: table => new
                {
                    dcckt = table.Column<string>(type: "text", nullable: false),
                    idc = table.Column<int>(type: "integer", nullable: false),
                    jdc = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ldc = table.Column<double>(type: "double precision", nullable: true),
                    met = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    rdc = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ntermdclink", x => new { x.dcckt, x.idc, x.jdc, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Ntermdclink_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    iowner = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    owname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => new { x.iowner, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Owner_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    irate = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    desc = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => new { x.irate, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Rating_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solver",
                columns: table => new
                {
                    method = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    actaps = table.Column<int>(type: "integer", nullable: true),
                    areain = table.Column<int>(type: "integer", nullable: true),
                    dctaps = table.Column<int>(type: "integer", nullable: true),
                    flatst = table.Column<int>(type: "integer", nullable: true),
                    nondiv = table.Column<int>(type: "integer", nullable: true),
                    phshft = table.Column<int>(type: "integer", nullable: true),
                    swshnt = table.Column<int>(type: "integer", nullable: true),
                    varlim = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solver", x => new { x.method, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Solver_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sub",
                columns: table => new
                {
                    isub = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    lati = table.Column<double>(type: "double precision", nullable: true),
                    @long = table.Column<double>(name: "long", type: "double precision", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    srg = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub", x => new { x.isub, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Sub_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subswd",
                columns: table => new
                {
                    inode = table.Column<int>(type: "integer", nullable: false),
                    isub = table.Column<int>(type: "integer", nullable: false),
                    jnode = table.Column<int>(type: "integer", nullable: false),
                    swdid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    nstat = table.Column<int>(type: "integer", nullable: true),
                    rate1 = table.Column<double>(type: "double precision", nullable: true),
                    rate2 = table.Column<double>(type: "double precision", nullable: true),
                    rate3 = table.Column<double>(type: "double precision", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    xpu = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subswd", x => new { x.inode, x.isub, x.jnode, x.swdid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Subswd_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Twotermdc",
                columns: table => new
                {
                    ici = table.Column<int>(type: "integer", nullable: false),
                    icr = table.Column<int>(type: "integer", nullable: false),
                    idi = table.Column<string>(type: "text", nullable: false),
                    idr = table.Column<string>(type: "text", nullable: false),
                    ifi = table.Column<int>(type: "integer", nullable: false),
                    ifr = table.Column<int>(type: "integer", nullable: false),
                    ipi = table.Column<int>(type: "integer", nullable: false),
                    ipr = table.Column<int>(type: "integer", nullable: false),
                    iti = table.Column<int>(type: "integer", nullable: false),
                    itr = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    nbr = table.Column<int>(type: "integer", nullable: false),
                    ndi = table.Column<int>(type: "integer", nullable: false),
                    ndr = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    anmni = table.Column<double>(type: "double precision", nullable: true),
                    anmnr = table.Column<double>(type: "double precision", nullable: true),
                    anmxi = table.Column<double>(type: "double precision", nullable: true),
                    anmxr = table.Column<double>(type: "double precision", nullable: true),
                    cccacc = table.Column<double>(type: "double precision", nullable: true),
                    cccitmx = table.Column<int>(type: "integer", nullable: true),
                    dcvmin = table.Column<double>(type: "double precision", nullable: true),
                    delti = table.Column<double>(type: "double precision", nullable: true),
                    ebasi = table.Column<double>(type: "double precision", nullable: true),
                    ebasr = table.Column<double>(type: "double precision", nullable: true),
                    mdc = table.Column<int>(type: "integer", nullable: true),
                    met = table.Column<string>(type: "text", nullable: true),
                    nbi = table.Column<int>(type: "integer", nullable: true),
                    rci = table.Column<double>(type: "double precision", nullable: true),
                    rcomp = table.Column<double>(type: "double precision", nullable: true),
                    rcr = table.Column<double>(type: "double precision", nullable: true),
                    rdc = table.Column<double>(type: "double precision", nullable: true),
                    setvl = table.Column<double>(type: "double precision", nullable: true),
                    stpi = table.Column<double>(type: "double precision", nullable: true),
                    stpr = table.Column<double>(type: "double precision", nullable: true),
                    tapi = table.Column<double>(type: "double precision", nullable: true),
                    tapr = table.Column<double>(type: "double precision", nullable: true),
                    tmni = table.Column<double>(type: "double precision", nullable: true),
                    tmnr = table.Column<double>(type: "double precision", nullable: true),
                    tmxi = table.Column<double>(type: "double precision", nullable: true),
                    tmxr = table.Column<double>(type: "double precision", nullable: true),
                    tri = table.Column<double>(type: "double precision", nullable: true),
                    trr = table.Column<double>(type: "double precision", nullable: true),
                    vcmod = table.Column<double>(type: "double precision", nullable: true),
                    vschd = table.Column<double>(type: "double precision", nullable: true),
                    xcapi = table.Column<double>(type: "double precision", nullable: true),
                    xcapr = table.Column<double>(type: "double precision", nullable: true),
                    xci = table.Column<double>(type: "double precision", nullable: true),
                    xcr = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Twotermdc", x => new { x.ici, x.icr, x.idi, x.idr, x.ifi, x.ifr, x.ipi, x.ipr, x.iti, x.itr, x.name, x.nbr, x.ndi, x.ndr, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Twotermdc_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tysl",
                columns: table => new
                {
                    itmxty = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    accty = table.Column<double>(type: "double precision", nullable: true),
                    tolty = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tysl", x => new { x.itmxty, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Tysl_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                columns: table => new
                {
                    izone = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    zoname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => new { x.izone, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Zone_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Iatransfer",
                columns: table => new
                {
                    arfrom = table.Column<int>(type: "integer", nullable: false),
                    arto = table.Column<int>(type: "integer", nullable: false),
                    trid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    FromAreaCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ToAreaCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ptran = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iatransfer", x => new { x.arfrom, x.arto, x.trid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Iatransfer_Area_arfrom_FromAreaCaseNumber",
                        columns: x => new { x.arfrom, x.FromAreaCaseNumber },
                        principalTable: "Area",
                        principalColumns: new[] { "iarea", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Iatransfer_Area_arto_ToAreaCaseNumber",
                        columns: x => new { x.arto, x.ToAreaCaseNumber },
                        principalTable: "Area",
                        principalColumns: new[] { "iarea", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Iatransfer_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vscdc",
                columns: table => new
                {
                    ibus1 = table.Column<int>(type: "integer", nullable: false),
                    ibus2 = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    acset1 = table.Column<double>(type: "double precision", nullable: true),
                    acset2 = table.Column<double>(type: "double precision", nullable: true),
                    aloss1 = table.Column<double>(type: "double precision", nullable: true),
                    aloss2 = table.Column<double>(type: "double precision", nullable: true),
                    bloss1 = table.Column<double>(type: "double precision", nullable: true),
                    bloss2 = table.Column<double>(type: "double precision", nullable: true),
                    dcset1 = table.Column<double>(type: "double precision", nullable: true),
                    dcset2 = table.Column<double>(type: "double precision", nullable: true),
                    f1 = table.Column<double>(type: "double precision", nullable: true),
                    f2 = table.Column<double>(type: "double precision", nullable: true),
                    f3 = table.Column<double>(type: "double precision", nullable: true),
                    f4 = table.Column<double>(type: "double precision", nullable: true),
                    imax1 = table.Column<double>(type: "double precision", nullable: true),
                    imax2 = table.Column<double>(type: "double precision", nullable: true),
                    maxq1 = table.Column<double>(type: "double precision", nullable: true),
                    maxq2 = table.Column<double>(type: "double precision", nullable: true),
                    mdc = table.Column<int>(type: "integer", nullable: true),
                    minloss1 = table.Column<double>(type: "double precision", nullable: true),
                    minloss2 = table.Column<double>(type: "double precision", nullable: true),
                    minq1 = table.Column<double>(type: "double precision", nullable: true),
                    minq2 = table.Column<double>(type: "double precision", nullable: true),
                    mode1 = table.Column<int>(type: "integer", nullable: true),
                    mode2 = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    nreg1 = table.Column<int>(type: "integer", nullable: true),
                    nreg2 = table.Column<int>(type: "integer", nullable: true),
                    o1 = table.Column<int>(type: "integer", nullable: true),
                    Owner1CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o2 = table.Column<int>(type: "integer", nullable: true),
                    Owner2CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o3 = table.Column<int>(type: "integer", nullable: true),
                    Owner3CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o4 = table.Column<int>(type: "integer", nullable: true),
                    Owner4CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    pwf1 = table.Column<double>(type: "double precision", nullable: true),
                    pwf2 = table.Column<double>(type: "double precision", nullable: true),
                    rdc = table.Column<double>(type: "double precision", nullable: true),
                    rmpct1 = table.Column<double>(type: "double precision", nullable: true),
                    rmpct2 = table.Column<double>(type: "double precision", nullable: true),
                    smax1 = table.Column<double>(type: "double precision", nullable: true),
                    smax2 = table.Column<double>(type: "double precision", nullable: true),
                    type1 = table.Column<int>(type: "integer", nullable: true),
                    type2 = table.Column<int>(type: "integer", nullable: true),
                    vsreg1 = table.Column<int>(type: "integer", nullable: true),
                    vsreg2 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vscdc", x => new { x.ibus1, x.ibus2, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Vscdc_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vscdc_Owner_o1_Owner1CaseNumber",
                        columns: x => new { x.o1, x.Owner1CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vscdc_Owner_o2_Owner2CaseNumber",
                        columns: x => new { x.o2, x.Owner2CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vscdc_Owner_o3_Owner3CaseNumber",
                        columns: x => new { x.o3, x.Owner3CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vscdc_Owner_o4_Owner4CaseNumber",
                        columns: x => new { x.o4, x.Owner4CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bus",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    iarea = table.Column<int>(type: "integer", nullable: true),
                    baskv = table.Column<double>(type: "double precision", nullable: true),
                    evhi = table.Column<double>(type: "double precision", nullable: true),
                    evlo = table.Column<double>(type: "double precision", nullable: true),
                    ide = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    nvhi = table.Column<double>(type: "double precision", nullable: true),
                    nvlo = table.Column<double>(type: "double precision", nullable: true),
                    iowner = table.Column<int>(type: "integer", nullable: true),
                    va = table.Column<double>(type: "double precision", nullable: true),
                    vm = table.Column<double>(type: "double precision", nullable: true),
                    zone = table.Column<int>(type: "integer", nullable: true),
                    Izone = table.Column<int>(type: "integer", nullable: true),
                    AreaCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    OwnerCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ZoneCaseNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bus", x => new { x.ibus, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Bus_Area_iarea_AreaCaseNumber",
                        columns: x => new { x.iarea, x.AreaCaseNumber },
                        principalTable: "Area",
                        principalColumns: new[] { "iarea", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bus_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bus_Owner_iowner_OwnerCaseNumber",
                        columns: x => new { x.iowner, x.OwnerCaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bus_Zone_Izone_ZoneCaseNumber",
                        columns: x => new { x.Izone, x.ZoneCaseNumber },
                        principalTable: "Zone",
                        principalColumns: new[] { "izone", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ntermdcbus",
                columns: table => new
                {
                    ib = table.Column<int>(type: "integer", nullable: false),
                    idc = table.Column<int>(type: "integer", nullable: false),
                    idc2 = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    iarea = table.Column<int>(type: "integer", nullable: true),
                    dcname = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    iowner = table.Column<int>(type: "integer", nullable: true),
                    rgrnd = table.Column<double>(type: "double precision", nullable: true),
                    zone = table.Column<int>(type: "integer", nullable: true),
                    ZoneIzone = table.Column<int>(type: "integer", nullable: true),
                    ZoneCaseNumber = table.Column<int>(type: "integer", nullable: true),
                    AreaCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    OwnerCaseNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ntermdcbus", x => new { x.ib, x.idc, x.idc2, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Ntermdcbus_Area_iarea_AreaCaseNumber",
                        columns: x => new { x.iarea, x.AreaCaseNumber },
                        principalTable: "Area",
                        principalColumns: new[] { "iarea", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ntermdcbus_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ntermdcbus_Owner_iowner_OwnerCaseNumber",
                        columns: x => new { x.iowner, x.OwnerCaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ntermdcbus_Zone_ZoneIzone_ZoneCaseNumber",
                        columns: x => new { x.ZoneIzone, x.ZoneCaseNumber },
                        principalTable: "Zone",
                        principalColumns: new[] { "izone", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Acline",
                columns: table => new
                {
                    ckt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    jbus = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    bi = table.Column<double>(type: "double precision", nullable: true),
                    bj = table.Column<double>(type: "double precision", nullable: true),
                    bpu = table.Column<double>(type: "double precision", nullable: true),
                    f1 = table.Column<double>(type: "double precision", nullable: true),
                    f2 = table.Column<double>(type: "double precision", nullable: true),
                    f3 = table.Column<double>(type: "double precision", nullable: true),
                    f4 = table.Column<double>(type: "double precision", nullable: true),
                    gi = table.Column<double>(type: "double precision", nullable: true),
                    gj = table.Column<double>(type: "double precision", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ToBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    len = table.Column<double>(type: "double precision", nullable: true),
                    met = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    o1 = table.Column<int>(type: "integer", nullable: true),
                    Owner1CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o2 = table.Column<int>(type: "integer", nullable: true),
                    Owner2CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o3 = table.Column<int>(type: "integer", nullable: true),
                    Owner3CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o4 = table.Column<int>(type: "integer", nullable: true),
                    Owner4CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    rate1 = table.Column<double>(type: "double precision", nullable: true),
                    rate10 = table.Column<double>(type: "double precision", nullable: true),
                    rate11 = table.Column<double>(type: "double precision", nullable: true),
                    rate12 = table.Column<double>(type: "double precision", nullable: true),
                    rate2 = table.Column<double>(type: "double precision", nullable: true),
                    rate3 = table.Column<double>(type: "double precision", nullable: true),
                    rate4 = table.Column<double>(type: "double precision", nullable: true),
                    rate5 = table.Column<double>(type: "double precision", nullable: true),
                    rate6 = table.Column<double>(type: "double precision", nullable: true),
                    rate7 = table.Column<double>(type: "double precision", nullable: true),
                    rate8 = table.Column<double>(type: "double precision", nullable: true),
                    rate9 = table.Column<double>(type: "double precision", nullable: true),
                    rpu = table.Column<double>(type: "double precision", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    xpu = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acline", x => new { x.ckt, x.ibus, x.jbus, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Acline_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acline_Bus_jbus_ToBusCaseNumber",
                        columns: x => new { x.jbus, x.ToBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acline_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acline_Owner_o1_Owner1CaseNumber",
                        columns: x => new { x.o1, x.Owner1CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acline_Owner_o2_Owner2CaseNumber",
                        columns: x => new { x.o2, x.Owner2CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acline_Owner_o3_Owner3CaseNumber",
                        columns: x => new { x.o3, x.Owner3CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acline_Owner_o4_Owner4CaseNumber",
                        columns: x => new { x.o4, x.Owner4CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facts",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    jbus = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    fcreg = table.Column<int>(type: "integer", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    imx = table.Column<double>(type: "double precision", nullable: true),
                    ToBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    linx = table.Column<double>(type: "double precision", nullable: true),
                    mname = table.Column<string>(type: "text", nullable: true),
                    mode = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    nreg = table.Column<int>(type: "integer", nullable: true),
                    iowner = table.Column<int>(type: "integer", nullable: true),
                    pdes = table.Column<double>(type: "double precision", nullable: true),
                    qdes = table.Column<double>(type: "double precision", nullable: true),
                    rmpct = table.Column<double>(type: "double precision", nullable: true),
                    set1 = table.Column<double>(type: "double precision", nullable: true),
                    set2 = table.Column<double>(type: "double precision", nullable: true),
                    shmx = table.Column<double>(type: "double precision", nullable: true),
                    trmx = table.Column<double>(type: "double precision", nullable: true),
                    vset = table.Column<double>(type: "double precision", nullable: true),
                    vsmx = table.Column<double>(type: "double precision", nullable: true),
                    vsref = table.Column<int>(type: "integer", nullable: true),
                    vtmn = table.Column<double>(type: "double precision", nullable: true),
                    vtmx = table.Column<double>(type: "double precision", nullable: true),
                    OwnerCaseNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facts", x => new { x.ibus, x.jbus, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Facts_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facts_Bus_jbus_ToBusCaseNumber",
                        columns: x => new { x.jbus, x.ToBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facts_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facts_Owner_iowner_OwnerCaseNumber",
                        columns: x => new { x.iowner, x.OwnerCaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fixshunt",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    shntid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    bl = table.Column<double>(type: "double precision", nullable: true),
                    gl = table.Column<double>(type: "double precision", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    stat = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixshunt", x => new { x.ibus, x.shntid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Fixshunt_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fixshunt_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Generator",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    machid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    baslod = table.Column<int>(type: "integer", nullable: true),
                    droopname = table.Column<string>(type: "text", nullable: true),
                    f1 = table.Column<double>(type: "double precision", nullable: true),
                    f2 = table.Column<string>(type: "text", nullable: true),
                    f3 = table.Column<string>(type: "text", nullable: true),
                    f4 = table.Column<string>(type: "text", nullable: true),
                    gtap = table.Column<double>(type: "double precision", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ireg = table.Column<int>(type: "integer", nullable: true),
                    mbase = table.Column<double>(type: "double precision", nullable: true),
                    nreg = table.Column<int>(type: "integer", nullable: true),
                    o1 = table.Column<int>(type: "integer", nullable: true),
                    Owner1CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o2 = table.Column<int>(type: "integer", nullable: true),
                    Owner2CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o3 = table.Column<int>(type: "integer", nullable: true),
                    Owner3CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o4 = table.Column<int>(type: "integer", nullable: true),
                    Owner4CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    pb = table.Column<double>(type: "double precision", nullable: true),
                    pg = table.Column<double>(type: "double precision", nullable: true),
                    pt = table.Column<double>(type: "double precision", nullable: true),
                    qb = table.Column<double>(type: "double precision", nullable: true),
                    qg = table.Column<double>(type: "double precision", nullable: true),
                    qt = table.Column<double>(type: "double precision", nullable: true),
                    rmpct = table.Column<double>(type: "double precision", nullable: true),
                    rt = table.Column<double>(type: "double precision", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    vs = table.Column<double>(type: "double precision", nullable: true),
                    wmod = table.Column<int>(type: "integer", nullable: true),
                    wpf = table.Column<float>(type: "real", nullable: true),
                    xt = table.Column<double>(type: "double precision", nullable: true),
                    zr = table.Column<double>(type: "double precision", nullable: true),
                    zx = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generator", x => new { x.ibus, x.machid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Generator_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Generator_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Generator_Owner_o1_Owner1CaseNumber",
                        columns: x => new { x.o1, x.Owner1CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Generator_Owner_o2_Owner2CaseNumber",
                        columns: x => new { x.o2, x.Owner2CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Generator_Owner_o3_Owner3CaseNumber",
                        columns: x => new { x.o3, x.Owner3CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Generator_Owner_o4_Owner4CaseNumber",
                        columns: x => new { x.o4, x.Owner4CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Indmach",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    imid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    aconst = table.Column<double>(type: "double precision", nullable: true),
                    iarea = table.Column<int>(type: "integer", nullable: true),
                    bc = table.Column<int>(type: "integer", nullable: true),
                    bconst = table.Column<double>(type: "double precision", nullable: true),
                    dc = table.Column<int>(type: "integer", nullable: true),
                    dconst = table.Column<double>(type: "double precision", nullable: true),
                    e1 = table.Column<string>(type: "text", nullable: true),
                    e2 = table.Column<string>(type: "text", nullable: true),
                    econst = table.Column<double>(type: "double precision", nullable: true),
                    hconst = table.Column<double>(type: "double precision", nullable: true),
                    ia1 = table.Column<string>(type: "text", nullable: true),
                    ia2 = table.Column<string>(type: "text", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    mbase = table.Column<double>(type: "double precision", nullable: true),
                    iowner = table.Column<int>(type: "integer", nullable: true),
                    pcode = table.Column<int>(type: "integer", nullable: true),
                    pset = table.Column<double>(type: "double precision", nullable: true),
                    r1 = table.Column<string>(type: "text", nullable: true),
                    r2 = table.Column<string>(type: "text", nullable: true),
                    ra = table.Column<string>(type: "text", nullable: true),
                    ratekv = table.Column<double>(type: "double precision", nullable: true),
                    sc = table.Column<int>(type: "integer", nullable: true),
                    se1 = table.Column<string>(type: "text", nullable: true),
                    se2 = table.Column<string>(type: "text", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    tc = table.Column<int>(type: "integer", nullable: true),
                    x1 = table.Column<string>(type: "text", nullable: true),
                    x2 = table.Column<string>(type: "text", nullable: true),
                    x3 = table.Column<string>(type: "text", nullable: true),
                    xa = table.Column<string>(type: "text", nullable: true),
                    xamult = table.Column<string>(type: "text", nullable: true),
                    xm = table.Column<string>(type: "text", nullable: true),
                    zone = table.Column<int>(type: "integer", nullable: true),
                    Izone = table.Column<int>(type: "integer", nullable: true),
                    AreaCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    OwnerCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ZoneCaseNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indmach", x => new { x.ibus, x.imid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Indmach_Area_iarea_AreaCaseNumber",
                        columns: x => new { x.iarea, x.AreaCaseNumber },
                        principalTable: "Area",
                        principalColumns: new[] { "iarea", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Indmach_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Indmach_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Indmach_Owner_iowner_OwnerCaseNumber",
                        columns: x => new { x.iowner, x.OwnerCaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Indmach_Zone_Izone_ZoneCaseNumber",
                        columns: x => new { x.Izone, x.ZoneCaseNumber },
                        principalTable: "Zone",
                        principalColumns: new[] { "izone", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Load",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    loadid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    iarea = table.Column<int>(type: "integer", nullable: true),
                    dgenm = table.Column<int>(type: "integer", nullable: true),
                    dgenp = table.Column<double>(type: "double precision", nullable: true),
                    dgenq = table.Column<double>(type: "double precision", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    intrpt = table.Column<int>(type: "integer", nullable: true),
                    ip = table.Column<double>(type: "double precision", nullable: true),
                    iq = table.Column<double>(type: "double precision", nullable: true),
                    loadtype = table.Column<string>(type: "text", nullable: true),
                    iowner = table.Column<int>(type: "integer", nullable: true),
                    pl = table.Column<double>(type: "double precision", nullable: true),
                    ql = table.Column<double>(type: "double precision", nullable: true),
                    scale = table.Column<int>(type: "integer", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    yp = table.Column<double>(type: "double precision", nullable: true),
                    yq = table.Column<double>(type: "double precision", nullable: true),
                    zone = table.Column<int>(type: "integer", nullable: true),
                    Izone = table.Column<int>(type: "integer", nullable: true),
                    AreaCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    OwnerCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ZoneCaseNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Load", x => new { x.ibus, x.loadid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Load_Area_iarea_AreaCaseNumber",
                        columns: x => new { x.iarea, x.AreaCaseNumber },
                        principalTable: "Area",
                        principalColumns: new[] { "iarea", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Load_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Load_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Load_Owner_iowner_OwnerCaseNumber",
                        columns: x => new { x.iowner, x.OwnerCaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Load_Zone_Izone_ZoneCaseNumber",
                        columns: x => new { x.Izone, x.ZoneCaseNumber },
                        principalTable: "Zone",
                        principalColumns: new[] { "izone", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Msline",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    jbus = table.Column<int>(type: "integer", nullable: false),
                    mslid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    dum1 = table.Column<int>(type: "integer", nullable: true),
                    dum2 = table.Column<int>(type: "integer", nullable: true),
                    dum3 = table.Column<int>(type: "integer", nullable: true),
                    dum4 = table.Column<int>(type: "integer", nullable: true),
                    dum5 = table.Column<int>(type: "integer", nullable: true),
                    dum6 = table.Column<int>(type: "integer", nullable: true),
                    dum7 = table.Column<int>(type: "integer", nullable: true),
                    dum8 = table.Column<int>(type: "integer", nullable: true),
                    dum9 = table.Column<int>(type: "integer", nullable: true),
                    Dum1BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum2BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum3BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum4BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum5BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum6BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum7BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum8BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    Dum9BusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ToBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    met = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Msline", x => new { x.ibus, x.jbus, x.mslid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum1_Dum1BusCaseNumber",
                        columns: x => new { x.dum1, x.Dum1BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum2_Dum2BusCaseNumber",
                        columns: x => new { x.dum2, x.Dum2BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum3_Dum3BusCaseNumber",
                        columns: x => new { x.dum3, x.Dum3BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum4_Dum4BusCaseNumber",
                        columns: x => new { x.dum4, x.Dum4BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum5_Dum5BusCaseNumber",
                        columns: x => new { x.dum5, x.Dum5BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum6_Dum6BusCaseNumber",
                        columns: x => new { x.dum6, x.Dum6BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum7_Dum7BusCaseNumber",
                        columns: x => new { x.dum7, x.Dum7BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum8_Dum8BusCaseNumber",
                        columns: x => new { x.dum8, x.Dum8BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_dum9_Dum9BusCaseNumber",
                        columns: x => new { x.dum9, x.Dum9BusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_Bus_jbus_ToBusCaseNumber",
                        columns: x => new { x.jbus, x.ToBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Msline_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subnode",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    inode = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    isub = table.Column<int>(type: "integer", nullable: true),
                    FromSubCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    va = table.Column<string>(type: "text", nullable: true),
                    vm = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subnode", x => new { x.ibus, x.inode, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Subnode_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subnode_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subnode_Sub_isub_FromSubCaseNumber",
                        columns: x => new { x.isub, x.FromSubCaseNumber },
                        principalTable: "Sub",
                        principalColumns: new[] { "isub", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Swshunt",
                columns: table => new
                {
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    shntid = table.Column<string>(type: "text", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    adjm = table.Column<int>(type: "integer", nullable: true),
                    b1 = table.Column<double>(type: "double precision", nullable: true),
                    b2 = table.Column<double>(type: "double precision", nullable: true),
                    b3 = table.Column<double>(type: "double precision", nullable: true),
                    b4 = table.Column<double>(type: "double precision", nullable: true),
                    b5 = table.Column<double>(type: "double precision", nullable: true),
                    b6 = table.Column<double>(type: "double precision", nullable: true),
                    b7 = table.Column<double>(type: "double precision", nullable: true),
                    b8 = table.Column<double>(type: "double precision", nullable: true),
                    binit = table.Column<double>(type: "double precision", nullable: true),
                    modsw = table.Column<int>(type: "integer", nullable: true),
                    n1 = table.Column<int>(type: "integer", nullable: true),
                    n2 = table.Column<int>(type: "integer", nullable: true),
                    n3 = table.Column<int>(type: "integer", nullable: true),
                    n4 = table.Column<int>(type: "integer", nullable: true),
                    n5 = table.Column<int>(type: "integer", nullable: true),
                    n6 = table.Column<int>(type: "integer", nullable: true),
                    n7 = table.Column<int>(type: "integer", nullable: true),
                    n8 = table.Column<int>(type: "integer", nullable: true),
                    nreg = table.Column<int>(type: "integer", nullable: true),
                    rmidnt = table.Column<string>(type: "text", nullable: true),
                    rmpct = table.Column<double>(type: "double precision", nullable: true),
                    s1 = table.Column<int>(type: "integer", nullable: true),
                    s2 = table.Column<int>(type: "integer", nullable: true),
                    s3 = table.Column<int>(type: "integer", nullable: true),
                    s4 = table.Column<int>(type: "integer", nullable: true),
                    s5 = table.Column<int>(type: "integer", nullable: true),
                    s6 = table.Column<int>(type: "integer", nullable: true),
                    s7 = table.Column<int>(type: "integer", nullable: true),
                    s8 = table.Column<int>(type: "integer", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    swreg = table.Column<int>(type: "integer", nullable: true),
                    vswhi = table.Column<double>(type: "double precision", nullable: true),
                    vswlo = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Swshunt", x => new { x.ibus, x.shntid, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Swshunt_Bus_ibus_CaseNumber",
                        columns: x => new { x.ibus, x.CaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Swshunt_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sysswd",
                columns: table => new
                {
                    ckt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    jbus = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ToBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    met = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    nstat = table.Column<int>(type: "integer", nullable: true),
                    rate1 = table.Column<double>(type: "double precision", nullable: true),
                    rate10 = table.Column<double>(type: "double precision", nullable: true),
                    rate11 = table.Column<double>(type: "double precision", nullable: true),
                    rate12 = table.Column<double>(type: "double precision", nullable: true),
                    rate2 = table.Column<double>(type: "double precision", nullable: true),
                    rate3 = table.Column<double>(type: "double precision", nullable: true),
                    rate4 = table.Column<double>(type: "double precision", nullable: true),
                    rate5 = table.Column<double>(type: "double precision", nullable: true),
                    rate6 = table.Column<double>(type: "double precision", nullable: true),
                    rate7 = table.Column<double>(type: "double precision", nullable: true),
                    rate8 = table.Column<double>(type: "double precision", nullable: true),
                    rate9 = table.Column<double>(type: "double precision", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    stype = table.Column<int>(type: "integer", nullable: true),
                    xpu = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sysswd", x => new { x.ckt, x.ibus, x.jbus, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Sysswd_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sysswd_Bus_jbus_ToBusCaseNumber",
                        columns: x => new { x.jbus, x.ToBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sysswd_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transformer",
                columns: table => new
                {
                    ckt = table.Column<string>(type: "text", nullable: false),
                    ibus = table.Column<int>(type: "integer", nullable: false),
                    jbus = table.Column<int>(type: "integer", nullable: false),
                    kbus = table.Column<int>(type: "integer", nullable: false),
                    CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ang1 = table.Column<double>(type: "double precision", nullable: true),
                    ang2 = table.Column<string>(type: "text", nullable: true),
                    ang3 = table.Column<string>(type: "text", nullable: true),
                    anstar = table.Column<string>(type: "text", nullable: true),
                    cm = table.Column<int>(type: "integer", nullable: true),
                    cnxa1 = table.Column<double>(type: "double precision", nullable: true),
                    cnxa2 = table.Column<string>(type: "text", nullable: true),
                    cnxa3 = table.Column<string>(type: "text", nullable: true),
                    cod1 = table.Column<int>(type: "integer", nullable: true),
                    cod2 = table.Column<string>(type: "text", nullable: true),
                    cod3 = table.Column<string>(type: "text", nullable: true),
                    cont1 = table.Column<int>(type: "integer", nullable: true),
                    cont2 = table.Column<string>(type: "text", nullable: true),
                    cont3 = table.Column<string>(type: "text", nullable: true),
                    cr1 = table.Column<double>(type: "double precision", nullable: true),
                    cr2 = table.Column<string>(type: "text", nullable: true),
                    cr3 = table.Column<string>(type: "text", nullable: true),
                    cw = table.Column<int>(type: "integer", nullable: true),
                    cx1 = table.Column<double>(type: "double precision", nullable: true),
                    cx2 = table.Column<string>(type: "text", nullable: true),
                    cx3 = table.Column<string>(type: "text", nullable: true),
                    cz = table.Column<int>(type: "integer", nullable: true),
                    f1 = table.Column<double>(type: "double precision", nullable: true),
                    f2 = table.Column<double>(type: "double precision", nullable: true),
                    f3 = table.Column<double>(type: "double precision", nullable: true),
                    f4 = table.Column<double>(type: "double precision", nullable: true),
                    FromBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    ToBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    AuxiliaryBusCaseNumber = table.Column<int>(type: "integer", nullable: false),
                    mag1 = table.Column<double>(type: "double precision", nullable: true),
                    mag2 = table.Column<double>(type: "double precision", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    nmet = table.Column<int>(type: "integer", nullable: true),
                    node1 = table.Column<int>(type: "integer", nullable: true),
                    node2 = table.Column<string>(type: "text", nullable: true),
                    node3 = table.Column<string>(type: "text", nullable: true),
                    nomv1 = table.Column<double>(type: "double precision", nullable: true),
                    nomv2 = table.Column<double>(type: "double precision", nullable: true),
                    nomv3 = table.Column<string>(type: "text", nullable: true),
                    ntp1 = table.Column<int>(type: "integer", nullable: true),
                    ntp2 = table.Column<string>(type: "text", nullable: true),
                    ntp3 = table.Column<string>(type: "text", nullable: true),
                    o1 = table.Column<int>(type: "integer", nullable: true),
                    Owner1CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o2 = table.Column<int>(type: "integer", nullable: true),
                    Owner2CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o3 = table.Column<int>(type: "integer", nullable: true),
                    Owner3CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    o4 = table.Column<int>(type: "integer", nullable: true),
                    Owner4CaseNumber = table.Column<int>(type: "integer", nullable: false),
                    r1_2 = table.Column<double>(type: "double precision", nullable: true),
                    r2_3 = table.Column<string>(type: "text", nullable: true),
                    r3_1 = table.Column<string>(type: "text", nullable: true),
                    rma1 = table.Column<double>(type: "double precision", nullable: true),
                    rma2 = table.Column<string>(type: "text", nullable: true),
                    rma3 = table.Column<string>(type: "text", nullable: true),
                    rmi1 = table.Column<double>(type: "double precision", nullable: true),
                    rmi2 = table.Column<string>(type: "text", nullable: true),
                    rmi3 = table.Column<string>(type: "text", nullable: true),
                    sbase1_2 = table.Column<double>(type: "double precision", nullable: true),
                    sbase2_3 = table.Column<string>(type: "text", nullable: true),
                    sbase3_1 = table.Column<string>(type: "text", nullable: true),
                    stat = table.Column<int>(type: "integer", nullable: true),
                    tab1 = table.Column<int>(type: "integer", nullable: true),
                    tab2 = table.Column<string>(type: "text", nullable: true),
                    tab3 = table.Column<string>(type: "text", nullable: true),
                    vecgrp = table.Column<string>(type: "text", nullable: true),
                    vma1 = table.Column<double>(type: "double precision", nullable: true),
                    vma2 = table.Column<string>(type: "text", nullable: true),
                    vma3 = table.Column<string>(type: "text", nullable: true),
                    vmi1 = table.Column<double>(type: "double precision", nullable: true),
                    vmi2 = table.Column<string>(type: "text", nullable: true),
                    vmi3 = table.Column<string>(type: "text", nullable: true),
                    vmstar = table.Column<string>(type: "text", nullable: true),
                    wdg1rate1 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate10 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate11 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate12 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate2 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate3 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate4 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate5 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate6 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate7 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate8 = table.Column<double>(type: "double precision", nullable: true),
                    wdg1rate9 = table.Column<double>(type: "double precision", nullable: true),
                    wdg2rate1 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate10 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate11 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate12 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate2 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate3 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate4 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate5 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate6 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate7 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate8 = table.Column<string>(type: "text", nullable: true),
                    wdg2rate9 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate1 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate10 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate11 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate12 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate2 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate3 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate4 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate5 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate6 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate7 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate8 = table.Column<string>(type: "text", nullable: true),
                    wdg3rate9 = table.Column<string>(type: "text", nullable: true),
                    windv1 = table.Column<double>(type: "double precision", nullable: true),
                    windv2 = table.Column<double>(type: "double precision", nullable: true),
                    windv3 = table.Column<string>(type: "text", nullable: true),
                    x1_2 = table.Column<double>(type: "double precision", nullable: true),
                    x2_3 = table.Column<string>(type: "text", nullable: true),
                    x3_1 = table.Column<string>(type: "text", nullable: true),
                    zcod = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transformer", x => new { x.ckt, x.ibus, x.jbus, x.kbus, x.CaseNumber });
                    table.ForeignKey(
                        name: "FK_Transformer_Bus_ibus_FromBusCaseNumber",
                        columns: x => new { x.ibus, x.FromBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_Bus_jbus_ToBusCaseNumber",
                        columns: x => new { x.jbus, x.ToBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_Bus_kbus_AuxiliaryBusCaseNumber",
                        columns: x => new { x.kbus, x.AuxiliaryBusCaseNumber },
                        principalTable: "Bus",
                        principalColumns: new[] { "ibus", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_CaseInfo_CaseNumber",
                        column: x => x.CaseNumber,
                        principalTable: "CaseInfo",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_Owner_o1_Owner1CaseNumber",
                        columns: x => new { x.o1, x.Owner1CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_Owner_o2_Owner2CaseNumber",
                        columns: x => new { x.o2, x.Owner2CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_Owner_o3_Owner3CaseNumber",
                        columns: x => new { x.o3, x.Owner3CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transformer_Owner_o4_Owner4CaseNumber",
                        columns: x => new { x.o4, x.Owner4CaseNumber },
                        principalTable: "Owner",
                        principalColumns: new[] { "iowner", "CaseNumber" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acline_CaseNumber",
                table: "Acline",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Acline_ibus_FromBusCaseNumber",
                table: "Acline",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Acline_jbus_ToBusCaseNumber",
                table: "Acline",
                columns: new[] { "jbus", "ToBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Acline_o1_Owner1CaseNumber",
                table: "Acline",
                columns: new[] { "o1", "Owner1CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Acline_o2_Owner2CaseNumber",
                table: "Acline",
                columns: new[] { "o2", "Owner2CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Acline_o3_Owner3CaseNumber",
                table: "Acline",
                columns: new[] { "o3", "Owner3CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Acline_o4_Owner4CaseNumber",
                table: "Acline",
                columns: new[] { "o4", "Owner4CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Adjust_CaseNumber",
                table: "Adjust",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Area_CaseNumber",
                table: "Area",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Bus_CaseNumber",
                table: "Bus",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Bus_iarea_AreaCaseNumber",
                table: "Bus",
                columns: new[] { "iarea", "AreaCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Bus_iowner_OwnerCaseNumber",
                table: "Bus",
                columns: new[] { "iowner", "OwnerCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Bus_Izone_ZoneCaseNumber",
                table: "Bus",
                columns: new[] { "Izone", "ZoneCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Caseid_CaseNumber",
                table: "Caseid",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CaseInfo_CaseSetNum",
                table: "CaseInfo",
                column: "CaseSetNum");

            migrationBuilder.CreateIndex(
                name: "IX_Facts_CaseNumber",
                table: "Facts",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Facts_ibus_FromBusCaseNumber",
                table: "Facts",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Facts_iowner_OwnerCaseNumber",
                table: "Facts",
                columns: new[] { "iowner", "OwnerCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Facts_jbus_ToBusCaseNumber",
                table: "Facts",
                columns: new[] { "jbus", "ToBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Fixshunt_CaseNumber",
                table: "Fixshunt",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Fixshunt_ibus_FromBusCaseNumber",
                table: "Fixshunt",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Gauss_CaseNumber",
                table: "Gauss",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_General_CaseNumber",
                table: "General",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Generator_CaseNumber",
                table: "Generator",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Generator_ibus_FromBusCaseNumber",
                table: "Generator",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Generator_o1_Owner1CaseNumber",
                table: "Generator",
                columns: new[] { "o1", "Owner1CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Generator_o2_Owner2CaseNumber",
                table: "Generator",
                columns: new[] { "o2", "Owner2CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Generator_o3_Owner3CaseNumber",
                table: "Generator",
                columns: new[] { "o3", "Owner3CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Generator_o4_Owner4CaseNumber",
                table: "Generator",
                columns: new[] { "o4", "Owner4CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Iatransfer_arfrom_FromAreaCaseNumber",
                table: "Iatransfer",
                columns: new[] { "arfrom", "FromAreaCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Iatransfer_arto_ToAreaCaseNumber",
                table: "Iatransfer",
                columns: new[] { "arto", "ToAreaCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Iatransfer_CaseNumber",
                table: "Iatransfer",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Impcor_CaseNumber",
                table: "Impcor",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Indmach_CaseNumber",
                table: "Indmach",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Indmach_iarea_AreaCaseNumber",
                table: "Indmach",
                columns: new[] { "iarea", "AreaCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Indmach_ibus_FromBusCaseNumber",
                table: "Indmach",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Indmach_iowner_OwnerCaseNumber",
                table: "Indmach",
                columns: new[] { "iowner", "OwnerCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Indmach_Izone_ZoneCaseNumber",
                table: "Indmach",
                columns: new[] { "Izone", "ZoneCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Load_CaseNumber",
                table: "Load",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Load_iarea_AreaCaseNumber",
                table: "Load",
                columns: new[] { "iarea", "AreaCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Load_ibus_FromBusCaseNumber",
                table: "Load",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Load_iowner_OwnerCaseNumber",
                table: "Load",
                columns: new[] { "iowner", "OwnerCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Load_Izone_ZoneCaseNumber",
                table: "Load",
                columns: new[] { "Izone", "ZoneCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_CaseNumber",
                table: "Msline",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum1_Dum1BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum1", "Dum1BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum2_Dum2BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum2", "Dum2BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum3_Dum3BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum3", "Dum3BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum4_Dum4BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum4", "Dum4BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum5_Dum5BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum5", "Dum5BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum6_Dum6BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum6", "Dum6BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum7_Dum7BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum7", "Dum7BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum8_Dum8BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum8", "Dum8BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_dum9_Dum9BusCaseNumber",
                table: "Msline",
                columns: new[] { "dum9", "Dum9BusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_ibus_FromBusCaseNumber",
                table: "Msline",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Msline_jbus_ToBusCaseNumber",
                table: "Msline",
                columns: new[] { "jbus", "ToBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Newton_CaseNumber",
                table: "Newton",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdc_CaseNumber",
                table: "Ntermdc",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdcbus_CaseNumber",
                table: "Ntermdcbus",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdcbus_iarea_AreaCaseNumber",
                table: "Ntermdcbus",
                columns: new[] { "iarea", "AreaCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdcbus_iowner_OwnerCaseNumber",
                table: "Ntermdcbus",
                columns: new[] { "iowner", "OwnerCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdcbus_ZoneIzone_ZoneCaseNumber",
                table: "Ntermdcbus",
                columns: new[] { "ZoneIzone", "ZoneCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdcconv_CaseNumber",
                table: "Ntermdcconv",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Ntermdclink_CaseNumber",
                table: "Ntermdclink",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_CaseNumber",
                table: "Owner",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_CaseNumber",
                table: "Rating",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Solver_CaseNumber",
                table: "Solver",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_CaseNumber",
                table: "Sub",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Subnode_CaseNumber",
                table: "Subnode",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Subnode_ibus_FromBusCaseNumber",
                table: "Subnode",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Subnode_isub_FromSubCaseNumber",
                table: "Subnode",
                columns: new[] { "isub", "FromSubCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Subswd_CaseNumber",
                table: "Subswd",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Swshunt_CaseNumber",
                table: "Swshunt",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Swshunt_ibus_CaseNumber",
                table: "Swshunt",
                columns: new[] { "ibus", "CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Sysswd_CaseNumber",
                table: "Sysswd",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Sysswd_ibus_FromBusCaseNumber",
                table: "Sysswd",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Sysswd_jbus_ToBusCaseNumber",
                table: "Sysswd",
                columns: new[] { "jbus", "ToBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_CaseNumber",
                table: "Transformer",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_ibus_FromBusCaseNumber",
                table: "Transformer",
                columns: new[] { "ibus", "FromBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_jbus_ToBusCaseNumber",
                table: "Transformer",
                columns: new[] { "jbus", "ToBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_kbus_AuxiliaryBusCaseNumber",
                table: "Transformer",
                columns: new[] { "kbus", "AuxiliaryBusCaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_o1_Owner1CaseNumber",
                table: "Transformer",
                columns: new[] { "o1", "Owner1CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_o2_Owner2CaseNumber",
                table: "Transformer",
                columns: new[] { "o2", "Owner2CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_o3_Owner3CaseNumber",
                table: "Transformer",
                columns: new[] { "o3", "Owner3CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Transformer_o4_Owner4CaseNumber",
                table: "Transformer",
                columns: new[] { "o4", "Owner4CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Twotermdc_CaseNumber",
                table: "Twotermdc",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Tysl_CaseNumber",
                table: "Tysl",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Vscdc_CaseNumber",
                table: "Vscdc",
                column: "CaseNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Vscdc_o1_Owner1CaseNumber",
                table: "Vscdc",
                columns: new[] { "o1", "Owner1CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Vscdc_o2_Owner2CaseNumber",
                table: "Vscdc",
                columns: new[] { "o2", "Owner2CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Vscdc_o3_Owner3CaseNumber",
                table: "Vscdc",
                columns: new[] { "o3", "Owner3CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Vscdc_o4_Owner4CaseNumber",
                table: "Vscdc",
                columns: new[] { "o4", "Owner4CaseNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Zone_CaseNumber",
                table: "Zone",
                column: "CaseNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acline");

            migrationBuilder.DropTable(
                name: "Adjust");

            migrationBuilder.DropTable(
                name: "Caseid");

            migrationBuilder.DropTable(
                name: "Facts");

            migrationBuilder.DropTable(
                name: "Fixshunt");

            migrationBuilder.DropTable(
                name: "Gauss");

            migrationBuilder.DropTable(
                name: "General");

            migrationBuilder.DropTable(
                name: "Generator");

            migrationBuilder.DropTable(
                name: "Iatransfer");

            migrationBuilder.DropTable(
                name: "Impcor");

            migrationBuilder.DropTable(
                name: "Indmach");

            migrationBuilder.DropTable(
                name: "Load");

            migrationBuilder.DropTable(
                name: "Msline");

            migrationBuilder.DropTable(
                name: "Newton");

            migrationBuilder.DropTable(
                name: "Ntermdc");

            migrationBuilder.DropTable(
                name: "Ntermdcbus");

            migrationBuilder.DropTable(
                name: "Ntermdcconv");

            migrationBuilder.DropTable(
                name: "Ntermdclink");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "Solver");

            migrationBuilder.DropTable(
                name: "Subnode");

            migrationBuilder.DropTable(
                name: "Subswd");

            migrationBuilder.DropTable(
                name: "Swshunt");

            migrationBuilder.DropTable(
                name: "Sysswd");

            migrationBuilder.DropTable(
                name: "Transformer");

            migrationBuilder.DropTable(
                name: "Twotermdc");

            migrationBuilder.DropTable(
                name: "Tysl");

            migrationBuilder.DropTable(
                name: "Vscdc");

            migrationBuilder.DropTable(
                name: "Sub");

            migrationBuilder.DropTable(
                name: "Bus");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "Zone");

            migrationBuilder.DropTable(
                name: "CaseInfo");

            migrationBuilder.DropTable(
                name: "CaseSet");
        }
    }
}
