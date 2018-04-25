// see: http://frhagn.github.io/Typewriter/ 
// or: https://github.com/frhagn/Typewriter
${
    string ResourceName(Class c)
    {
        return c.Name.Replace("DTO", string.Empty) + "Resource";
    }
}
module Models { $Classes(*DTO)[ 
    export interface $Name$TypeParameters { 
        $save: Function;$Properties[
        $name: $Type;]
    }
    export interface $ResourceName {
        new (): $Name$TypeParameters;
        get: Function;
        delete: Function;
        query: Function;
        sort: Function;
    }
]
$Enums(*)[
    export enum $Name { $Values[
        $Name = $Value][,]
    }
    ]
}