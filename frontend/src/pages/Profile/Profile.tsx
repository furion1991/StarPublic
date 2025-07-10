'use client'

import { useState } from 'react'

import { UserAuthorizedMainInfo } from '@/widgets/user'
import { InventoryLootList } from '@/widgets/inventory'
import { UserHistoryTabs } from '@/entities/user'
import { Button } from '@/shared/ui'
import { InventoryLootActions } from '@/features/inventory'
import { UpgradesHistoryList } from '@/widgets/upgrades'
import { ContractsHistoryList } from '@/widgets/contracts'
import { WithdrawnItemsList } from '@/entities/withdrawals'

import { useUser } from '@/shared/hooks'

import classes from './Profile.module.scss'

type TabItem = {
  label: string
  value: string
  iconPath: string
  iconActivePath: string
}

export const ProfilePage = () => {
  const [tab, setTab] = useState('inventory')

  const tabs: TabItem[] = [
    {
      label: 'Мой инвентарь',
      value: 'inventory',
      iconPath: '/icons/chest-blue.svg',
      iconActivePath: '/icons/chest-gradient.svg'
    },
    {
      label: 'Апгрейды',
      value: 'upgrades',
      iconPath: '/icons/upgrade-blue.svg',
      iconActivePath: '/icons/upgrade-gradient.svg'
    },
    {
      label: 'Контракты',
      value: 'contracts',
      iconPath: '/icons/medal-blue.svg',
      iconActivePath: '/icons/medal-gradient.svg'
    },
    {
      label: 'Выводы',
      value: 'withdrawals',
      iconPath: '/icons/clock-blue.svg',
      iconActivePath: '/icons/clock-gradient.svg'
    }
  ]

  const { user, isUserLoading } = useUser()

  const userInventoryLootItems = user
    ? user.userInventory.itemsUserInventory
        .filter(({ itemDto }) => Boolean(itemDto))
        .map(({ id, itemDto }) => {
          if (!itemDto) {
            return {
              inventoryItemId: '',
              rarity: 0,
              game: '',
              name: '',
              sellPrice: 0,
              image: ''
            }
          }

          const { rarity, game, name, sellPrice, image } = itemDto

          return {
            inventoryItemId: id,
            rarity,
            game,
            name,
            sellPrice,
            image
          }
        })
    : []

  return (
    <div className={classes.wrapper}>
      <UserAuthorizedMainInfo />

      <div className={classes.userHistory}>
        <div className={classes.userHistoryTopPanel}>
          <UserHistoryTabs
            activeTab={tab}
            tabs={tabs}
            onTabChange={setTab}
            slots={{
              actions: <InventoryLootActions />
            }}
          />
        </div>

        {tab === 'inventory' ? (
          <InventoryLootList
            isLoading={isUserLoading}
            lootItems={userInventoryLootItems}
            lootItemActions={
              <div className={classes.lootItemActions}>
                <Button>Продать</Button>

                <Button color='purple'>Вывести</Button>
              </div>
            }
          />
        ) : null}

        {tab === 'upgrades' ? <UpgradesHistoryList /> : null}

        {tab === 'contracts' ? <ContractsHistoryList /> : null}

        {tab === 'withdrawals' ? <WithdrawnItemsList /> : null}
      </div>
    </div>
  )
}
