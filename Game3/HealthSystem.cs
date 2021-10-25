using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem{
    private int health;
    private int maxHealth;

    public HealthSystem(int health){
        this.health = health;
        this.maxHealth = health;
    }

    

    public float getHealth(){
        return (float)health;
    }

    public float getHealthPercentage(){
        return (float) health / maxHealth ;
    }

    public void damage(int damage , bool weak){
        if(weak){
            health -= damage;
        }else{
            health -= damage/2;
        }
        if (health < 0){
            health = 0;
        }

    }

    public void dodge(int damage){
        health -= damage * Random.Range(0,3)/10;
    }

    public void heal(int heal , bool success){
        if (!success){
            heal = heal/2;
        }
        health += heal;
        if(health > maxHealth){
            health = maxHealth;
        }

    }
}
