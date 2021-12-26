namespace AdventOfCode.Utils;

class OCR6x5 : OCR
{
    public override int CharWidth => 5;

    public override int CharHeight => 6;

    public override int Spacing => 0;

    protected override (string letters, string drawing) GetAlphabet() => ("ABCEFGHIJKLOPRSUYZ", @"
 ##  ###   ##  #### ####  ##  #  # ###    ## #  # #     ##  ###  ###   ### #  # #   ##### 
#  # #  # #  # #    #    #  # #  #  #      # # #  #    #  # #  # #  # #    #  # #   #   # 
#  # ###  #    ###  ###  #    ####  #      # ##   #    #  # #  # #  # #    #  #  # #   #  
#### #  # #    #    #    # ## #  #  #      # # #  #    #  # ###  ###   ##  #  #   #   #   
#  # #  # #  # #    #    #  # #  #  #   #  # # #  #    #  # #    # #     # #  #   #  #    
#  # ###   ##  #### #     ### #  # ###   ##  #  # ####  ##  #    #  # ###   ##    #  #### ");
}
