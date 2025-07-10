import Image from 'next/image'
import cn from 'classnames'

import { LootItem } from '@/entities/loot'

import type { UpgradeItem as UpgradeItemType } from '@/entities/user'

import classes from './UpgradeItem.module.scss'

export const UpgradeItem = ({
  itemUsedOnThisUpgrade,
  resultItem,
  chance,
  isSuccessful
}: UpgradeItemType) => {
  return (
    <div className={classes.upgradeItem}>
      <div className={classes.titles}>
        <div>
          <p className={classes.columnTitle}>Ваша ставка</p>
        </div>

        <div>
          <p className={classes.columnTitle}>Апгрейд</p>
        </div>
      </div>

      <div className={classes.items}>
        <LootItem
          className={classes.loot}
          image={itemUsedOnThisUpgrade.image}
          rarity={itemUsedOnThisUpgrade.rarity}
          price={itemUsedOnThisUpgrade.sellPrice}
          name={itemUsedOnThisUpgrade.name}
        />

        <Image
          src={
            isSuccessful ? '/icons/upgrade-double-success.svg' : '/icons/upgrade-double-failure.svg'
          }
          width={26}
          height={26}
          alt='Иконка'
        />

        <LootItem
          className={classes.loot}
          image={resultItem.image}
          rarity={resultItem.rarity}
          price={resultItem.sellPrice}
          name={resultItem.name}
        />
      </div>

      <div className={classes.upgradeInfo}>
        <div className={classes.chance}>
          <p>
            Шанс: <span className={classes.chanceValue}>{chance}%</span>
          </p>
        </div>

        <div
          className={cn(classes.result, {
            [classes.success]: isSuccessful,
            [classes.failure]: !isSuccessful
          })}
        >
          <p>{isSuccessful ? 'Выигрыш' : 'Проигрыш'}</p>
        </div>
      </div>
    </div>
  )
}
