'use client'

import { useState } from 'react'
import { FormProvider, useForm } from 'react-hook-form'

import { UserAuthorizedMainInfo } from '@/widgets/user'
import { InventoryLootList } from '@/widgets/inventory'
import { UserHistoryTabs } from '@/entities/user'
import { InventoryLootActions, useSellItems } from '@/features/inventory'
import { UpgradesHistoryList } from '@/widgets/upgrades'
import { ContractsHistoryList } from '@/widgets/contracts'
import { WithdrawnItemsList } from '@/entities/withdrawals'

import { useUser } from '@/shared/hooks'

import classes from './Profile.module.scss'

type FormProps = {
  canSellFilter: boolean
}

type TabItem = {
  label: string
  value: string
  iconPath: string
  iconActivePath: string
}

export const ProfilePage = () => {
  const [tab, setTab] = useState('inventory')
  const { user, isUserLoading } = useUser()
  const useFormProps = useForm<FormProps>({
    defaultValues: {
      canSellFilter: true
    }
  })
  const { watch } = useFormProps

  const { mutate: sellItems } = useSellItems()

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

  const inventoryLootItems = user
    ? watch('canSellFilter')
      ? user.userInventory.availableInventoryItems
      : user.userInventory.itemsUserInventory
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
              actions:
                tab === 'inventory' ? (
                  <FormProvider {...useFormProps}>
                    <InventoryLootActions />
                  </FormProvider>
                ) : null
            }}
          />
        </div>

        {tab === 'inventory' ? (
          <InventoryLootList
            isLoading={isUserLoading}
            lootItems={inventoryLootItems}
            onItemSell={(id) => {
              sellItems([id])
            }}
            onItemWithdraw={() => {}}
          />
        ) : null}

        {tab === 'upgrades' ? (
          <UpgradesHistoryList upgrades={user ? user.upgradeHistoryRecords : []} />
        ) : null}

        {tab === 'contracts' ? (
          <ContractsHistoryList contracts={user ? user.contractHistoryRecords : []} />
        ) : null}

        {tab === 'withdrawals' ? <WithdrawnItemsList /> : null}
      </div>
    </div>
  )
}
